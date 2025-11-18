using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    private float checkRate = 0.05f;
    private float lastCheckTime;
    [SerializeField] private float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObj;
    private IInteractable curInteractable;

    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private TextMeshProUGUI promptText;

    [SerializeField] private Camera camera;

    public Vector3 hitPosition;

    private void Start()
    {
        if(camera == null)
        {
            //camera = Camera.main;
        }
    }

    private void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject != curInteractGameObj)
                {
                    hitPosition = hit.point;
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
        switch (curInteractable.GetInteractableType())
        {
            case InteractableType.Item:
                interactionText.gameObject.SetActive(true);
                promptText.gameObject.SetActive(true);
                promptText.text = curInteractable.GetInteractPrompt();
                break;
            case InteractableType.Animal:
                promptText.gameObject.SetActive(true);
                promptText.text = curInteractable.GetInteractPrompt();
                break;
            case InteractableType.NPC:
                interactionText.gameObject.SetActive(true);
                promptText.gameObject.SetActive(true);
                promptText.text = curInteractable.GetInteractPrompt();
                break;
        }
    }

    // 상호작용 하는 키
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable != null)
        {
            switch (curInteractable.GetInteractableType())
            {
                case InteractableType.Item:
                    curInteractable.OnInteract();
                    curInteractGameObj = null;
                    curInteractable = null;
                    promptText.gameObject.SetActive(false);
                    break;
                case InteractableType.Animal:
                    Debug.Log("동물과 상호작용 했습니다.");
                    curInteractable.OnInteract();
                    curInteractGameObj = null;
                    curInteractable = null;
                    break;
                case InteractableType.NPC:
                    curInteractable.OnInteract();   // NPC클래스에서 구현
                    Debug.Log("NPC와 상호작용 했습니다.");
                    break;
            }
            
        }
    }
}
