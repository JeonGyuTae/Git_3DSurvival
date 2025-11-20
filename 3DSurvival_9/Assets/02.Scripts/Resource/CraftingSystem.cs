using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [Header("Recipes")]
    public List<RecipeData> recipes = new List<RecipeData>();

    //[Header("Inventory")]
    //[SerializeField] private PlayerInventory playerInventory;   // 인스펙터에서 Player Inventory 드래그

    //private void Start()
    //{
    //    // 인스펙터에서 안 넣어줬으면, 실행할 때 자동으로 찾기
    //    if (playerInventory == null)
    //    {
    //        playerInventory = PlayerManager.Instance.PlayerInventory;
    //    }
    //}

    public bool TryCraft(RecipeData recipe)
    {
        // 매번 PlayerManager에서 바로 꺼내쓰기
        PlayerInventory inv = PlayerManager.Instance.PlayerInventory;

        if (inv == null)
        {
            Debug.LogWarning("CraftingSystem: PlayerManager.Instance.PlayerInventory가 비어 있음");
            return false;
        }

        if (recipe == null)
        {
            Debug.LogWarning("CraftingSystem: recipe가 비어 있음");
            return false;
        }

        // 여기부터는 inv를 가지고 제작 진행

        // 1) 재료 있는지 확인
        foreach (var input in recipe.inputs)
        {
            if (!inv.HasItem(input.item, input.amount))
            {
                Debug.Log("재료 부족: " + input.item.name);
                return false;
            }
        }

        // 재료 소모
        foreach (var input in recipe.inputs)
        {
            inv.RemoveItem(input.item, input.amount);
        }

        // 3) 결과 지급
        inv.AddItem(recipe.outputItem, recipe.outputAmount);

        Debug.Log("제작 성공: " + recipe.outputItem.name);
        return true;
    }

}
