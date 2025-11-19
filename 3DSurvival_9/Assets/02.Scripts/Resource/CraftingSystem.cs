using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가공/제작 시스템의 뼈대
/// 실제 인벤토리에서 재료를 빼고, 결과 아이템을 넣는 부분은
/// 인벤토리 담당과 협의하여 채워 넣기
/// </summary>
public class CraftingSystem : MonoBehaviour
{
    [Header("Recipes")]
    public List<RecipeData> recipes = new List<RecipeData>();

    /// <summary>
    /// 특정 레시피를 이용해 제작을 시도
    /// 나중에 인벤토리 타입을 파라미터로 추가하면 된다.
    /// 예: public bool TryCraft(RecipeData recipe, PlayerInventory inventory)
    /// </summary>
    public bool TryCraft(RecipeData recipe)
    {
        if (recipe == null)
        {
            Debug.LogWarning("CraftingSystem: recipe가 null");
            return false;
        }

        // 1. 인벤토리에 inputA / inputB가 충분한지 확인
        // 2. 재료 소모
        // 3. outputItem 지급

        Debug.Log($"[CraftingSystem] {recipe.name} 제작 시도 (인벤토리 연동 전)");
        return false;
    }
}
