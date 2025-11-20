using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [Header("레시피 리스트")]
    public List<RecipeData> recipes = new List<RecipeData>();

    private PlayerInventory playerInventory;

    private void Awake()
    {
        // 씬에서 PlayerInventory 자동으로 찾기
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogWarning("[CraftingSystem] PlayerInventory를 찾지 못했습니다.");
            }
        }
    }

    /// <summary>
    /// 특정 레시피를 이용해 제작 시도.
    /// - 1. 모든 재료가 충분한지 확인
    /// - 2. 재료 소모
    /// - 3. 결과 아이템 지급
    /// </summary>
    public bool TryCraft(RecipeData recipe)
    {
        PlayerInventory inv = PlayerManager.Instance.PlayerInventory;

        if (inv == null)
        {
            Debug.LogWarning("CraftingSystem: PlayerInventory가 null임");
            return false;
        }

        if (recipe == null)
        {
            Debug.LogWarning("CraftingSystem: recipe가 null임");
            return false;
        }

        Debug.Log($"[TryCraft] {recipe.name} 제작 시도");

        // 1) 재료 체크
        foreach (var input in recipe.inputs)
        {
            bool has = inv.HasItem(input.item, input.amount);
            Debug.Log($"[TryCraft] 필요 재료: {input.item.name} x{input.amount} / 보유 여부: {has}");

            if (!has)
            {
                Debug.Log($"[TryCraft] 재료 부족 → 제작 실패 ({input.item.name})");
                return false;
            }
        }

        // 2) 재료 소모
        foreach (var input in recipe.inputs)
        {
            bool removed = inv.RemoveItem(input.item, input.amount);
            Debug.Log($"[TryCraft] 재료 소모: {input.item.name} x{input.amount} / 성공: {removed}");
        }

        // 3) 결과 지급
        bool added = inv.AddItem(recipe.outputItem, recipe.outputAmount);
        Debug.Log($"[TryCraft] 결과 지급: {recipe.outputItem.name} x{recipe.outputAmount} / 인벤토리 추가 성공 여부: {added}");

        return added;
    }

    /// <summary>
    /// 버튼에서 인덱스로 호출하고 싶을 때 쓸 수 있는 헬퍼 함수 (선택)
    /// </summary>
    public void TryCraftByIndex(int index)
    {
        if (index < 0 || index >= recipes.Count)
        {
            Debug.LogWarning($"[CraftingSystem] 잘못된 레시피 인덱스: {index}");
            return;
        }

        TryCraft(recipes[index]);
    }
}
