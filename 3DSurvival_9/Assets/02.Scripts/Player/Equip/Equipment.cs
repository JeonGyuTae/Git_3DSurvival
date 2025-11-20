using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerController controller;
    private PlayerCondition condition;

    [Header("Fist Attack")]
    [SerializeField] private float attackDamage; // 맨손 공격력
    [SerializeField] private float attackRate;  // 맨손 공격속도
    private bool attacking;
    [SerializeField] private float attackDistance;  // 맨손 공격 사거리
    [SerializeField] private float lastAttackTime;
    [SerializeField] private float useStamina;      // 맨손 공격 사용 스태미나
    [SerializeField] private LayerMask attackableLayer;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if (curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && controller.canLook)
        {
            if (curEquip != null)
            {
                curEquip.OnAttackInput();
            }
            else
            {
                FistAttack();
            }

        }
    }

    private void FistAttack()
    {
        if (Time.time >= lastAttackTime + attackRate)
        {
            if (PlayerManager.Instance.Player.condition.UseStamina(useStamina))
            {
                Debug.Log("맨손 공격");
                lastAttackTime = Time.time;

                RaycastHit[] hits = Physics.SphereCastAll(
                    transform.position + transform.forward * 0.5f,
                    attackDistance / 2,
                    transform.forward,
                    attackDistance,
                    attackableLayer
                );

                foreach (RaycastHit hit in hits)
                {
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage((int)attackDamage, hit.point);
                        break;
                    }
                }
            }
        }
    }
}
