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
    public bool canLook { get; private set; }
    [SerializeField] private Image crossHair;

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

    public System.Action inventory;
    public System.Action throwItem;
    public System.Action useItem;

    private PlayerCondition condition;
    private Equipment equipment;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        condition = GetComponent<PlayerCondition>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        equipment = GetComponent<Equipment>();

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
        if (crossHair != null)
        {
            crossHair.gameObject.SetActive(true);
        }
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

    #region 이동
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
    #endregion

    #region 마우스 시점
    void PlayerLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        camObj.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    } 
    #endregion

    #region 이동입력
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
    #endregion

    #region 웅크리기
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
    #endregion

    #region 달리기
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
    #endregion

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

    #region 점프
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (IsGrounded() && condition.UseStamina(useJumpStamina))
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
                isJumping = true;
            }
        }
    } 
    #endregion

    void GravityEffect()
    {
        if (!IsGrounded() && _rigidbody.velocity.y < 0)
        {
            _rigidbody.AddForce(Vector3.up * Physics.gravity.y * (fallingForce - 1) * _rigidbody.mass, ForceMode.Force);
        }
    }

    #region 땅 감지
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
    #endregion

    /// <summary>
    /// 인벤토리로 사용X 건축에서 사용될 예정
    /// </summary>
    /// <param name="context"></param>
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
        if (crossHair != null)
        {
            crossHair.gameObject.SetActive(canLook);
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            useItem?.Invoke();
        }
    }

    #region 아이템 슬롯 선택
    public void OnSelectSlot1(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(0);
            }
        }
    }

    public void OnSelectSlot2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(1);
            }
        }
    }

    public void OnSelectSlot3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(2);
            }
        }
    }

    public void OnSelectSlot4(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(3);
            }
        }
    }

    public void OnSelectSlot5(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(4);
            }
        }
    }

    public void OnSelectSlot6(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(5);
            }
        }
    }

    public void OnSelectSlot7(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(6);
            }
        }
    }

    public void OnSelectSlot8(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(7);
            }
        }
    }

    public void OnSelectSlot9(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance.PlayerInventory != null)
            {
                PlayerManager.Instance.PlayerInventory.EquipItemInSlot(8);
            }
        }
    }
    #endregion
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            throwItem?.Invoke();
        }
    }
}
