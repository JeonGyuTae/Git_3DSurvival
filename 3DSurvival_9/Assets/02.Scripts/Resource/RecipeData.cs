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
    public ResourceData inputA;
    public int inputCountA = 1;

    public ResourceData inputB;
    public int inputCountB = 0;

    [Header("Output")]
    public ResourceData output;
    public int outputCount = 1;
}