using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ResourceData",
    menuName = "Game/Resource Data",
    order = 0)]
public class ResourceData : ScriptableObject
{
    [Header("Info")]
    public string id;              // "branch" 등
    public string displayName;     // "나뭇가지"
    public Sprite icon;

    [Header("Stack")]
    public int maxStack = 99;

    [Header("Spawn")]
    [Tooltip("이 자원이 다시 스폰되기까지 걸리는 시간(초)")]
    public float respawnSeconds = 600f;

    [Header("Inventory Link")]
    [Tooltip("이 자원을 주웠을 때 인벤토리에 들어갈 ItemData")]
    public ItemData itemData;      
}
