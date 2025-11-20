using UnityEngine;

public class HarvestableNode : MonoBehaviour, IDamageable, ICullable
{
    [Header("Drop Item")]
    public ItemData dropItem;
    public int baseAmount = 1;

    [Header("Tool Bonus")]
    public float toolYieldMultiplier = 1f;

    [Header("Durability")]
    public int hitsToBreak = 3;

    private int _currentHits;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        DisableCullComponents();
    }

    public void DisableCullComponents()
    {
        meshRenderer.enabled = false;
    }

    public void EnableCullComponents()
    {
        meshRenderer.enabled = true;
    }

    // ���� ���� �� �Ѵ�. �׳� ������ ĳ��.
    public void Harvest()
    {
        Debug.Log("캐기");

        if (dropItem == null)
        {
            Debug.LogWarning($"{name} : dropItem�� ��� ����");
            return;
        }

        _currentHits++;
        Debug.Log($"{name} ����! ���� ��Ʈ ��: {_currentHits}/{hitsToBreak}");

        if (_currentHits >= hitsToBreak)
        {
            int amount = Mathf.RoundToInt(baseAmount * toolYieldMultiplier);
            if (amount <= 0) amount = 1;

            var inventory = PlayerManager.Instance.PlayerInventory;
            if (inventory != null)
            {
                inventory.AddItem(dropItem, amount);
                Debug.Log($"[HarvestableNode] {dropItem.name} x{amount} �κ��丮�� �߰�");
            }

            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        Harvest();
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        Harvest();
    }
}
