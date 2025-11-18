using UnityEngine;

[CreateAssetMenu(
    fileName = "NPCData",
    menuName = "Game/NPC Data",
    order = 0)]

public class NPCData : ScriptableObject
{
    [Header("Info")]
    public string npcName;
    public string displayName;

    [TextArea]
    public string[] dialogues;
}
