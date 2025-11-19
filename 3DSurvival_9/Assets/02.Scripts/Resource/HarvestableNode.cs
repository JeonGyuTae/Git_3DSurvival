using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableNode : MonoBehaviour
{
    [Header("Drop Item")]
    public ItemData dropItem;
    public int baseAmount = 1;

    [Header("Tool Bonus")]
    public float toolYieldMultiplier = 2f;

    [Header("Durability")]
    public int hitsToBreak = 3;

    private int _currentHits;

    public void Harvest(GameObject interactor, bool hasProperTool)
    {
        if (dropItem == null)
        {
            Debug.LogWarning($"{name} : dropItem이 비어 있음");
            return;
        }

        if (!hasProperTool)
        {
            Debug.Log($"[HarvestableNode] 잘못된 도구로 채집 시도: {dropItem.itemname}");
            return;
        }

        int amount = Mathf.RoundToInt(baseAmount * toolYieldMultiplier);
        if (amount <= 0) amount = 1;

        Debug.Log($"[HarvestableNode] {dropItem.itemname} x{amount} 채집 성공");

        _currentHits++;
        if (_currentHits >= hitsToBreak)
        {
            Destroy(gameObject);
        }
    }
}
