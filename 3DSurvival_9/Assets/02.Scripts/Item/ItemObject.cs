using UnityEngine;


public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData data;

    public InteractableType GetInteractableType()
    {
        return InteractableType.Item;
    }

    public string GetInteractPrompt()
    {
        string str = $"{data.itemname}";
        return str;
    }

    public void HideInteractUI()
    {
        
    }

    public void OnInteract()
    {
        PlayerManager.Instance.Player.itemData = data;
        PlayerManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }

    public void ShowInteractUI()
    {
        
    }
}
