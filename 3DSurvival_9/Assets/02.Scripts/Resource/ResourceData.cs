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
    public string id;
    public string displayName;

    [Header("UI")]
    public Sprite icon;

    [Header("Stack")]
    public int maxStack = 99;

    [Header("Spawn")]
    [Tooltip("이 자원이 다시 스폰되기까지 걸리는 시간(초)")]
    public float respawnSeconds = 600f;   // 10분
}
