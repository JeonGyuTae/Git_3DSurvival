using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "ScriptableObjects/Recipe")]
public class RecipeData : ScriptableObject
{
    [System.Serializable]
    public struct RecipeInput
    {
        public ItemData item;   // 필요한 재료 아이템
        public int amount;      // 필요한 개수
    }

    [Header("재료들")]
    public RecipeInput[] inputs;

    [Header("결과 아이템")]
    public ItemData outputItem;   // 도끼, 해머 같은 결과물
    public int outputAmount = 1;  // 결과 개수 (기본 1개)
}
