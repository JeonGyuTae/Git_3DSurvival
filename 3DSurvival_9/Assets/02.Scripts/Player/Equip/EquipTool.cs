using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;
    public float useStamina;

    [Header("Combat")]
    public int damage;

    private Animator animator;
    [SerializeField] private Camera camera;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (camera == null)
        {
            camera = Camera.main;
        }
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            if (PlayerManager.Instance.Player.condition.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate);
            }
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();

            if(damageable != null)
            {
                damageable.TakeDamage((int)damage);
            }
        }
    }
}
