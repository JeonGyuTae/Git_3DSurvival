using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableNode : MonoBehaviour, ICullable
{
    [Header("Drop Item")]
    public ItemData dropItem;
    public int baseAmount = 1;

    [Header("Tool Bonus")]
    public float toolYieldMultiplier = 2f;

    [Header("Durability")]
    public int hitsToBreak = 3;

    [Header("Required Tool")]
    public ToolType requiredTool = ToolType.None;

    private int _currentHits;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        DisableCullComponents();
    }

    public void EnableCullComponents()
    {
        meshRenderer.enabled = true;
    }

    public void DisableCullComponents()
    {
        meshRenderer.enabled = false;
    }

    public void Harvest(GameObject interactor, ToolType usedTool)
    {
        if (dropItem == null)
        {
            Debug.LogWarning($"{name} : dropItem이 비어 있음");
            return;
        }

        // 도구가 없거나, 잘못된 도구면 실패
        if (usedTool == ToolType.None || usedTool != requiredTool)
        {
            Debug.Log($"[HarvestableNode] 잘못된 도구로 채집 시도 ({usedTool} vs {requiredTool})");
            return;
        }

        int amount = Mathf.RoundToInt(baseAmount * toolYieldMultiplier);
        if (amount <= 0) amount = 1;

        // 인벤토리에 드랍 아이템 추가
        var inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(dropItem, amount);
        }

        _currentHits++;
        if (_currentHits >= hitsToBreak)
        {
            // 파괴 이펙트/사운드
            Destroy(gameObject);
        }
    }
}
