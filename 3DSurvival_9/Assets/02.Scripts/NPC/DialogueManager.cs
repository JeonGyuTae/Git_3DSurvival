using UnityEngine;
using TMPro;
// using UnityEngine.UI;        // 이미지 파일이 들어갈 경우 적용

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialogueUI;   // 대화 UI 패널
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Player")]
    public Transform playerTransform;     // 플레이어 Transform 연결
    public float rotateSpeed = 5f;        // 플레이어 회전 속도

    private string[] currentDialogues;
    private int index;

    private Transform lookTargetNPC;      // 현재 바라봐야 할 NPC Transform

    void Awake()
    {
        Instance = this;
    }



    // -- -- -- -- -- 대화 시작 -- -- -- -- --

    public void StartDialogue(NPCData data, Transform npcTransform)
    {
        dialogueUI.SetActive(true);

        nameText.text = data.npcName;

        currentDialogues = data.dialogues;
        index = 0;

        lookTargetNPC = npcTransform;     // NPC Transform 저장 (회전 타겟)

        ShowSentence();
    }



    // -- -- -- -- -- 다음 문장 -- -- -- -- --

    public void Next()
    {
        index++;

        if (index < currentDialogues.Length)
            ShowSentence();
        else
            EndDialogue();
    }



    // -- -- -- -- -- 문장 출력 -- -- -- -- --

    private void ShowSentence()
    {
        dialogueText.text = currentDialogues[index];
    }



    // -- -- -- -- -- 대화 종료 -- -- -- -- --

    public void EndDialogue()
    {
        dialogueUI.SetActive(false);

        lookTargetNPC = null;       // 회전 종료
    }



    // -- -- -- -- -- Update_Player 회전 처리 -- -- -- -- --

    private void Update()
    {
        RotatePlayerSmoothly();
    }



    // -- -- -- -- -- 부드러운 회전 (Slerp) -- -- -- -- --

    private void RotatePlayerSmoothly()
    {
        if (lookTargetNPC == null) return;

        Vector3 direction = (lookTargetNPC.position - playerTransform.position);
        direction.y = 0f; // 위아래 각도는 무시

        if (direction.sqrMagnitude < 0.001f) return; // 0에 가까운 경우 회전 없음

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        playerTransform.rotation =
            Quaternion.Slerp(playerTransform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
    }
}
