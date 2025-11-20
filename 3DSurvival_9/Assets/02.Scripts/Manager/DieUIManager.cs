using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DieUIManager : MonoBehaviour
{
    public static DieUIManager Instance { get; private set; }

    [Header("Die UI")]
    [SerializeField] private GameObject diePanel;
    [SerializeField] private Button respawnBtn;
    [SerializeField] private TextMeshProUGUI dieMessageText;

    private PlayerCondition playerCondition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (diePanel != null)
        {
            diePanel.SetActive(false);
        }
    }

    private void Start()
    {
        playerCondition = PlayerManager.Instance.Player.condition;

        if (respawnBtn != null)
        {
            respawnBtn.onClick.AddListener(OnRespawnBtnClicked);
        }
    }

    public void ShowDieScreen()
    {
        if (diePanel != null)
        {
            diePanel.SetActive(true);
        }
    }

    private void OnRespawnBtnClicked()
    {
        if (playerCondition != null)
        {
            playerCondition.Respawn();
        }
        HideDieScreen();
    }

    public void HideDieScreen()
    {
        if (diePanel != null)
        {
            diePanel.SetActive(false);
        }
    }
}