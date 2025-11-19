using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RecipeData",
    menuName = "Game/Recipe Data",
    order = 1)]
public class RecipeData : ScriptableObject
{
    [Header("Inputs")]
    public ItemData inputA;
    public int inputCountA = 1;

    public ItemData inputB;
    public int inputCountB = 0;

    [Header("Output")]
    public ItemData outputItem;      // µµ³¢, °î±ªÀÌ, °¡°ø ÀÚ¿ø µî
    public int outputItemCount = 1;  // º¸Åë 1
}