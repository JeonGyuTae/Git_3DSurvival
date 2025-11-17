using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialogueUI;   // 대화 UI 패널
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    private string[] currentDialogue;
    private int index = 0;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(NPCData data)
    {
        dialogueUI.SetActive(true);
        nameText.text = data.npcName;

        currentDialogue = data.dialogues;
        index = 0;

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (index >= currentDialogue.Length)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = currentDialogue[index];
        index++;
    }

    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
    }
}
