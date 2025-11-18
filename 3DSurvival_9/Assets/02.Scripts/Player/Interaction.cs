using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    private float checkRate = 0.05f;
    private float lastCheckTime;
    private float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObj;
    private IInteractable curInteractable;

    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private TextMeshProUGUI promptText;

    [SerializeField] private Camera camera;

    private void Start()
    {
        maxCheckDistance = Define.PLAYER_MAX_CHECK_RAY_DISTANCE;
    }

    private void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObj)
                {
                    curInteractGameObj = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObj = null;
                curInteractable = null;
                interactionText.gameObject.SetActive(false);
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        interactionText.gameObject.SetActive(true);
        promptText.gameObject.SetActive(true);
        // 상호작용이 가능한 개체만 InteractionText.gameObject.SetActive(true);
        // interactionText.text = curInteractable.GetInteractPrompt(); // 고정된 텍스트만 띄울 것
        promptText.text = curInteractable.GetInteractPrompt();
    }

    // 아이템 습득하는 키
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable != null) // 아이템만 인식하는 로직 추가
        {
            curInteractable.OnInteract();
            curInteractable = null;
            curInteractable = null;
        }
    }
}
