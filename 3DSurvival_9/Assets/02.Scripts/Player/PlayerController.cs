using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sneakingSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallingForce;
    [SerializeField] public float useRunStamina;
    [SerializeField] private float useJumpStamina;
    private Vector2 curMovementInput;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float sneakingColliderHeight;
    [SerializeField] private float sneakingCameraYOffset;

    [Header("Look")]
    [SerializeField] private Transform camObj;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    private float camCurXRot;
    [SerializeField] float lookSensitivity;
    private Vector2 mouseDelta;
    private bool canLook;
    [SerializeField] private Image crossHair;

    // 아직 정상작동 x
    [Header("Ground Snapping")]
    [SerializeField] private float snapToGroundDistance;
    [SerializeField] private float snapForce;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;

    private Rigidbody _rigidbody;
    private bool isMoving;
    private bool isSneaking;
    public bool isRunning { get; private set; }
    private bool isJumping;
    private bool runInputHold;

    private CapsuleCollider _capsuleCollider;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private Vector3 originalCamLocalPos;
    private float sneakingColliderCenterY;

    public Action inventory;

    private PlayerCondition condition;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        condition = GetComponent<PlayerCondition>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // 웅크리기시 콜라이더와 시점 조절
        if (_capsuleCollider != null )
        {
            originalColliderHeight = _capsuleCollider.height;
            originalColliderCenter = _capsuleCollider.center;

            sneakingColliderCenterY = originalColliderCenter.y - (originalColliderHeight - sneakingColliderHeight) / 2f;
        }

        if (camObj != null)
        {
            originalCamLocalPos = camObj.localPosition;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        canLook = true;
    }

    private void Update()
    {
        isMoving = curMovementInput.magnitude > 0.1f;
        isRunning = runInputHold && isMoving && !isSneaking;
    }

    private void FixedUpdate()
    {
        Move();
        GravityEffect();
        HandleSlopeSliding();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            PlayerLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;

        float speed;

        if (isRunning)
        {
            speed = runSpeed;
        }
        else if (isSneaking)
        {
            speed = sneakingSpeed;
        }
        else
        {
            speed = moveSpeed;
        }

        dir *= speed;

        // 경사면을 올라갈 때 불필요한 Y 가속도 방지 (이전 논의에서 결정된 로직)
        float currentYVelocity = _rigidbody.velocity.y;
        if (IsGrounded() && !isJumping && currentYVelocity > 0.01f)
        {
            currentYVelocity = 0f;
        }
        dir.y = currentYVelocity; // Rigidbody의 현재 Y 속도 유지 또는 위에서 조정한 값 적용

        _rigidbody.velocity = dir;

        // 점프 최고점 도달 후 isJumping 플래그 해제 로직
        if (isJumping && _rigidbody.velocity.y <= 0.01f)
        {
            isJumping = false;
        }
    }

    void PlayerLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        camObj.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isMoving = true;
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isMoving = false;
            curMovementInput = Vector2.zero;
        }
    }

    public void OnSneaking(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isSneaking = true;

            if (_capsuleCollider != null)
            {
                _capsuleCollider.height = sneakingColliderHeight;
                _capsuleCollider.center = new Vector3(originalColliderCenter.x, sneakingColliderCenterY, originalColliderCenter.z);
            }
            if (camObj != null)
            {
                camObj.localPosition = new Vector3(originalCamLocalPos.x, originalCamLocalPos.y + sneakingCameraYOffset, originalCamLocalPos.z);
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isSneaking = false;

            if(_capsuleCollider != null)
            {
                _capsuleCollider.height = originalColliderHeight;
                _capsuleCollider.center = originalColliderCenter;
            }
            if(camObj != null)
            {
                camObj.localPosition = originalCamLocalPos;
            }
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (isSneaking)
        {
            runInputHold = false;
            return;
        }

        if (context.phase == InputActionPhase.Performed && isMoving)
        {
            runInputHold = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            runInputHold = false;
        }
    }

    // 외부 접근용(PlayerCondition.cs)
    public void runInputHoldFalse()
    {
        if (runInputHold)
        {
            runInputHold = false;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if(IsGrounded() && condition.UseStamina(useJumpStamina))
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
                isJumping = true;
            }
        }
    }

    void GravityEffect()
    {
        if (!IsGrounded() && _rigidbody.velocity.y < 0)
        {
            _rigidbody.AddForce(Vector3.up * Physics.gravity.y * (fallingForce - 1) * _rigidbody.mass, ForceMode.Force);
        }
    }

    // 경사면 이동 시 (아직 정상작동x)
    void ApplyGroundSnapping()
    {
        if (IsGrounded() || _rigidbody.velocity.y > 0.1f)
        {
            return;
        }

        RaycastHit hit;

        Vector3 rayStart = transform.position + _capsuleCollider.center;
        rayStart.y = transform.position.y + _capsuleCollider.center.y - _capsuleCollider.height / 2f + 0.1f;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, snapToGroundDistance, groundLayerMask))
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
            _rigidbody.AddForce(Vector3.down * snapForce, ForceMode.Force);
        }
    }

    bool IsGrounded()
    {
        float capsuleBottomY = _capsuleCollider.bounds.center.y - _capsuleCollider.bounds.extents.y;

        float rayStartYOffset = 0.01f;
        Vector3 rayStartBase = new Vector3(transform.position.x, capsuleBottomY + rayStartYOffset, transform.position.z);

        float rayLength = 0.2f;
        float offsetFromEdge = _capsuleCollider.radius * 0.9f;

        Ray[] rays = new Ray[4]
        {
            new Ray(rayStartBase + transform.forward * offsetFromEdge, Vector3.down),
            new Ray(rayStartBase - transform.forward * offsetFromEdge, Vector3.down),
            new Ray(rayStartBase + transform.right * offsetFromEdge, Vector3.down),
            new Ray(rayStartBase - transform.right * offsetFromEdge, Vector3.down)
        };

        bool grounded = false;
        for (int i = 0; i < rays.Length; i++)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction * rayLength, Color.green, Time.fixedDeltaTime);

            if (Physics.Raycast(rays[i], rayLength, groundLayerMask))
            {
                grounded = true;
            }
        }
        return grounded;
    }

    private void HandleSlopeSliding()
    {
        if (!IsGrounded()) { Debug.Log("Not Grounded"); return; }
        if (isMoving) { Debug.Log("Is Moving"); return; }
        if (isJumping) { Debug.Log("Is Jumping"); return; }
        if (_rigidbody.velocity.magnitude >= 0.1f) { Debug.Log($"Velocity too high: {_rigidbody.velocity.magnitude}"); return; }

        if (IsGrounded() && !isMoving && !isJumping && _rigidbody.velocity.magnitude < 0.1f)
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + _capsuleCollider.center;
            rayOrigin.y -= (_capsuleCollider.height / 2f - 0.05f);

            Debug.DrawRay(rayOrigin, Vector3.down * 0.2f, Color.magenta, Time.fixedDeltaTime);

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 0.2f, groundLayerMask))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                if(slopeAngle <= maxSlopeAngle)
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                }
                else
                {
                    Debug.Log($"Slope too steep: {slopeAngle} > {maxSlopeAngle}");
                }
            }
        }
        else
        {
            Debug.Log("HandleSlopeSliding: Raycast did not hit ground!");
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }
    
    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
