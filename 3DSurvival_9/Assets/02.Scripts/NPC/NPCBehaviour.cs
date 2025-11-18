using UnityEngine;

/// <summary>
/// Player가 상호작용 가능한 NPC 오브젝트.
/// 팀 공통 E키 상호작용 시스템에서 Interact()를 호출해 주면
/// 해당 NPC와 대화가 가능하다.
/// </summary>
public class NPCBehaviour : MonoBehaviour
{
    public NPCData npcData;

    // Player가 상호작용할 때 호출하는 함수
    public NPCData GetNPCData()
    {
        return npcData;
    }
}

/*   상호작용 키를 직접 받는 버전
public class NPCBehaviour : MonoBehaviour
{
    public NPCData npcData;     // ScriptableObject 데이터 연결
    private bool isPlayerNearby;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        DialogueManager.Instance.StartDialogue(npcData);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}
 */
