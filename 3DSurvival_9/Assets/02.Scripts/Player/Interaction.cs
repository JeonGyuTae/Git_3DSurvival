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

    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
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
                    // IInteractable 구현 세부사항대로 프롬프트 활성/비활성화 수정
                    SetPromptText();
                }
            }
            else
            {
                curInteractable.HideInteractUI();
                curInteractGameObj = null;
                curInteractable = null;
            }
        }
    }

    private void SetPromptText()
    {
        curInteractable.ShowInteractUI();
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractable = null;
            curInteractable = null;
            curInteractable.HideInteractUI();
        }
    }
}
