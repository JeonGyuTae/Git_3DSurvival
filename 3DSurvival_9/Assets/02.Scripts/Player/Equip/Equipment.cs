using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;          // 현재 손에 든 무기(도끼, 해머 등)
    public Transform equipParent;   // 손 위치(무기 붙는 위치)

    private PlayerController controller;
    private PlayerCondition condition;

    [Header("Fist Attack")]         // 맨손 공격용 설정
    [SerializeField] private float attackDamage = 5f;    // 맨손 공격력
    [SerializeField] private float attackRate = 0.5f;    // 맨손 공격 속도
    private bool attacking;
    [SerializeField] private float attackDistance = 2f;  // 맨손 공격 사거리
    [SerializeField] private float lastAttackTime;
    [SerializeField] private float useStamina = 5f;      // 맨손 공격 스태미나 소모
    [SerializeField] private LayerMask attackableLayer;  // 공격 가능한 레이어

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }

    // 인벤토리에서 아이템 장착할 때 호출
    public void EquipNew(ItemData data)
    {
        UnEquip();

        if (data == null || data.equipPrefab == null)
        {
            Debug.LogWarning($"Equipment: {data?.name} 의 equipPrefab이 비어 있어서 장비 불가");
            return;
        }

        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
    }

    // 장비 해제
    public void UnEquip()
    {
        if (curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    // 공격 입력 (Input System에서 이 함수에 연결)
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && controller.canLook)
        {
            // 무기가 있으면 무기 공격
            if (curEquip != null)
            {
                curEquip.OnAttackInput();
            }
            // 없으면 맨손 공격
            else
            {
                FistAttack();
            }
        }
    }

    // 맨손 공격 로직
    private void FistAttack()
    {
        // 공격 쿨타임 체크
        if (Time.time < lastAttackTime + attackRate)
            return;

        // 스태미나 소비 실패하면 공격 취소
        if (!condition.UseStamina(useStamina))
            return;

        lastAttackTime = Time.time;
        Debug.Log("맨손 공격!");

        // 플레이어 앞 방향으로 스피어캐스트
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position + transform.forward * 0.5f,   // 시작 위치
            attackDistance / 2f,                             // 반지름
            transform.forward,                               // 방향
            attackDistance,
            attackableLayer                                  // 공격 가능한 레이어
        );

        foreach (RaycastHit hit in hits)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage((int)attackDamage, hit.point);
                break; // 한 번 맞추면 끝
            }
        }
    }
}
