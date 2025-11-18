using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 가공/제작 시스템의 뼈대임
/// 지금은 구조만 잡아 두고, 실제 인벤토리 연동은 나중에 채워 넣으면 됨
public class CraftingSystem : MonoBehaviour
{
    [Header("Recipes")]
    public List<RecipeData> recipes = new List<RecipeData>();

    public bool TryCraft(RecipeData recipe, Inventory inventory)
    {
        if (recipe == null || inventory == null)
        {
            Debug.LogWarning("CraftingSystem: recipe 또는 inventory가 null");
            return false;
        }

        // TODO:
        // 1. inventory에 필요한 재료가 충분한지 확인
        // 2. 재료 소모 후 output 아이템 AddItem
        // 현재는 구조만 있는 상태

        Debug.Log($"[CraftingSystem] {recipe.name} 제작 시도 (현재는 구조만 구현됨)");
        return false;
    }
}

