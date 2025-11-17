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

    [Header("Ground Snapping")]
    [SerializeField] private float snapToGroundDistance;
    [SerializeField] private float snapForce;

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
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;

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
    public void isRunningFalse()
    {
        if (isRunning)
        {
            isRunning = false;
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
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
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
