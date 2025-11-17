using UnityEngine;

/// <summary>
/// Player가 상호작용 가능한 NPC 오브젝트.
/// 팀 공통 E키 상호작용 시스템에서 Interact()를 호출해 주면
/// 해당 NPC와 대화가 가능하다.
/// </summary>
public class NPCController : MonoBehaviour
{
    public NPCData data;

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(data);
    }
}
