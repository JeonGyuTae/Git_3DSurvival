using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Transform slotPanel;
    private Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;

    [SerializeField] private GameObject useButton;
    [SerializeField] private GameObject equipButton;

    private PlayerController controller;
    private PlayerCondition condition;

    private ItemData selectedItem;
    private int selectedItemIndex = 0;

    private void Start()
    {
        controller = PlayerManager.Instance.Player.controller;
        condition = PlayerManager.Instance.Player.condition;
        dropPosition = PlayerManager.Instance.Player.dropPosition;

        controller.inventory += ToggleInventory;
        controller.throwItem += OnDropItem;
        PlayerManager.Instance.Player.addItem += AddItem;

        inventoryUI.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }
    }

    public void AddItem()
    {
        ItemData data = PlayerManager.Instance.Player.itemData;

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                PlayerManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.itemdata = data;
            emptySlot.quantity = 1;
            UpdateUI();
            PlayerManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        PlayerManager.Instance.Player.itemData = null;
    }

    public void ToggleInventory()
    {
        if (IsOpen())
        {
            inventoryUI.SetActive(false);
        }
        else
        {
            inventoryUI.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryUI.activeInHierarchy;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemdata != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemdata == data && slots[i].quantity < data.maxStack)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemdata == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].itemdata == null) return;

        selectedItem = slots[index].itemdata;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.itemname;

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable);
    }

    public void OnDropItem()
    {
        if (selectedItem != null)
        {
            ThrowItem(selectedItem);
            RemoveSelectedItem();
        }
    }
    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].itemdata = null;
            selectedItemIndex = -1;
        }

        UpdateUI();
    }
}
