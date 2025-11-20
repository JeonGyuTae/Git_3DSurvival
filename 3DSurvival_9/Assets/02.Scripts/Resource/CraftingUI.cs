using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    public CraftingSystem craftingSystem;
    public RecipeData recipeAxe;
    public RecipeData recipeHammer;

    public void OnClickCraftAxe()
    {
        craftingSystem.TryCraft(recipeAxe);
    }

    public void OnClickCraftHammer()
    {
        craftingSystem.TryCraft(recipeHammer);
    }
}

