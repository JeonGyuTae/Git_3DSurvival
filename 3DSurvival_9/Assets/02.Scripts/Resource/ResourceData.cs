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
    [Tooltip("�� �ڿ��� �ٽ� �����Ǳ���� �ɸ��� �ð�(��)")]
    public float respawnSeconds = 600f;   // 10��
}
