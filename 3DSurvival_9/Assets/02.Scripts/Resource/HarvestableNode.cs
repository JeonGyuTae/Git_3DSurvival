using UnityEngine;

public class HarvestableNode : MonoBehaviour
{
    [Header("Drop Item")]
    public ItemData dropItem;
    public int baseAmount = 1;

    [Header("Tool Bonus")]
    public float toolYieldMultiplier = 1f;

    [Header("Durability")]
    public int hitsToBreak = 3;

    private int _currentHits;

    // 도구 구분 안 한다. 그냥 맞으면 캐짐.
    public void Harvest()
    {
        if (dropItem == null)
        {
            Debug.LogWarning($"{name} : dropItem이 비어 있음");
            return;
        }

        _currentHits++;
        Debug.Log($"{name} 맞음! 현재 히트 수: {_currentHits}/{hitsToBreak}");

        if (_currentHits >= hitsToBreak)
        {
            int amount = Mathf.RoundToInt(baseAmount * toolYieldMultiplier);
            if (amount <= 0) amount = 1;

            var inventory = PlayerManager.Instance.PlayerInventory;
            if (inventory != null)
            {
                inventory.AddItem(dropItem, amount);
                Debug.Log($"[HarvestableNode] {dropItem.name} x{amount} 인벤토리에 추가");
            }

            Destroy(gameObject);
        }
    }
}
