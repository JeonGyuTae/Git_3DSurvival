using UnityEngine;

public class ResourcePickup : MonoBehaviour, IInteractable
{
    [Header("Resource")]
    [SerializeField] private ResourceData resourceData;
    [SerializeField] private int amount = 1;

    // 이 오브젝트는 어떤 타입의 인터랙션인지 (아이템, 동물, NPC...)
    public InteractableType GetInteractableType() => InteractableType.Item;

    // 화면에 뜰 이름 (E키 팝업에 나오는 텍스트)
    public string GetInteractPrompt()
    {
        if (resourceData == null)
            return "알 수 없는 자원";

        // ResourceData에 displayName 있으면 그걸 쓰고,
        // 없으면 연결된 ItemData의 이름을 써도 됨
        if (resourceData.itemData != null &&
            !string.IsNullOrEmpty(resourceData.itemData.itemname))
        {
            return resourceData.itemData.itemname;
        }

        return resourceData.displayName;
    }

    //실제로 E키 눌렀을 때 실행되는 로직
    public void OnInteract()
    {
        if (resourceData == null)
        {
            Debug.LogWarning($"{name} : ResourceData가 비어 있습니다.");
            return;
        }

        if (resourceData.itemData == null)
        {
            Debug.LogWarning($"{name} : ResourceData({resourceData.displayName})에 연결된 ItemData가 없습니다.");
            return;
        }

        // 팀이 만들어둔 인벤토리 구조 그대로 사용
        PlayerManager.Instance.Player.itemData = resourceData.itemData;
        PlayerManager.Instance.Player.addItem?.Invoke();

        //사운드, 이펙트 있으면 여기에서 재생
        Destroy(gameObject);
    }

    //  IInteractable 규칙상 필요한 함수 (Interaction에서 쓸 수도 있고, 안 쓸 수도 있음)

    public void ShowInteractUI()
    {
        // UI
    }
    public void HideInteractUI()
    {
        // Interaction 스크립트가 호출할 수 있는 'UI 숨기기'용 훅
        // 지금 당장은 비워둬도 상관 없음
    }
}
