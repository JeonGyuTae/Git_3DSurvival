using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData itemdata;

    public PlayerInventory inventory;

    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    public int index;
    public bool equipped;
    public int quantity;

    public void Set()
    {
        itemIcon.gameObject.SetActive(true);
        itemIcon.sprite = itemdata.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;
    }

    public void Clear()
    {
        itemdata = null;
        itemIcon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }
}
