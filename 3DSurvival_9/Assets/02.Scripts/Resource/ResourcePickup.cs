using UnityEngine;

public class ResourcePickup : MonoBehaviour, IInteractable
{
    [Header("Resource")]
    [SerializeField] private ResourceData resourceData;
    [SerializeField] private int amount = 1;

    public InteractableType GetInteractableType() => InteractableType.Item;

    public string GetInteractPrompt()
    {
        if (resourceData == null)
            return "알 수 없는 자원";

        return resourceData.displayName;
    }

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

        PlayerManager.Instance.Player.itemData = resourceData.itemData;
        PlayerManager.Instance.Player.addItem?.Invoke();

        Destroy(gameObject);
    }
    public void ShowInteractUI()
    {
        // UI
    }

    public void HideInteractUI()
    {
        // UI
    }
}
