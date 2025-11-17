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
                // curInteractable.HideInteractUI(); // 정보 UI활성화 하는거 여기서 관리해야할듯
                curInteractGameObj = null;
                curInteractable = null;
                interactionText.gameObject.SetActive(false);
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        // curInteractable.ShowInteractUI();
        interactionText.gameObject.SetActive(true);
        promptText.gameObject.SetActive(true);
        // interactionText.text = curInteractable.GetInteractPrompt(); // 아이템, 적에 따라서 상호작용 텍스트 변경되어야함
        promptText.text = curInteractable.GetInteractPrompt();
    }

    // 아이템 습득하는 키
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable != null) // 아이템만 검사하는 로직 추가할 것
        {
            curInteractable.OnInteract();
            curInteractable = null;
            curInteractable = null;
        }
    }
}
