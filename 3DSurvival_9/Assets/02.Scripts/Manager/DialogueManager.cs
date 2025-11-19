using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialogueUI;   // 대화 UI 패널
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Player")]
    public Transform playerTransform;
    public float rotateSpeed = 5f;

    private string[] currentDialogues;
    private int index;
    private Transform lookTargetNPC;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Player player = PlayerManager.Instance.Player;
        if (player != null)
            playerTransform = player.transform;
    }

    // 대화 시작
    public void StartDialogue(NPCData data, Transform npcTransform)
    {
        dialogueUI.SetActive(true);
        nameText.text = data.npcName;
        currentDialogues = data.dialogues;
        index = 0;
        lookTargetNPC = npcTransform;

        ShowSentence();
    }

    // 다음 문장
    public void Next()
    {
        index++;
        if (index < currentDialogues.Length)
            ShowSentence();
        else
            EndDialogue();
    }

    // 문장 출력
    private void ShowSentence()
    {
        dialogueText.text = currentDialogues[index];
    }

    // 대화 종료
    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
        lookTargetNPC = null;
    }

    // Update: 회전 + 입력 처리
    private void Update()
    {
        RotatePlayerSmoothly();

        if (dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            Next();
        }
    }

    // 부드러운 회전 (RotateTowards)
    private void RotatePlayerSmoothly()
    {
        if (lookTargetNPC == null) return;

        Vector3 direction = (lookTargetNPC.position - playerTransform.position);
        direction.y = 0f; // 위아래 각도는 무시

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float step = rotateSpeed * Time.deltaTime;
        playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, targetRotation, step);
    }
}
