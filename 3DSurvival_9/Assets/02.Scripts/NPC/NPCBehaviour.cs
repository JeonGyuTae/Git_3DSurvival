using UnityEngine;

/// <summary>
/// Player가 상호작용 가능한 NPC 오브젝트.
/// 팀 공통 E키 상호작용 시스템에서 Interact()를 호출해 주면
/// 해당 NPC와 대화가 가능하다.
/// </summary>
public class NPCBehaviour : MonoBehaviour, IInteractable
{
    public NPCData npcData;

    // IInteractable 인터페이스 구현

    public InteractableType GetInteractableType()
    {
        return InteractableType.NPC; // 이 오브젝트는 NPC임
    }

    public string GetInteractPrompt()
    {
        return npcData.displayName; // NPC 이름
    }

    public void ShowInteractUI()
    {

    }

    public void HideInteractUI()
    {

    }

    public void OnInteract()
    {
        // NPC와 상호작용 시 대화 시작
        DialogueManager.Instance.StartDialogue(npcData, transform);
        Debug.Log("상호작용 시도됨");
    }
}
