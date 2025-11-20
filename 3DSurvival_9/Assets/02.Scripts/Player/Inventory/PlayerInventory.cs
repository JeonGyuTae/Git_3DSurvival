using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    [SerializeField] private Transform slotPanel;
    private Transform dropPosition;

    private PlayerController controller;
    private PlayerCondition condition;

    private ItemData selectedItem;
    private int selectedItemIndex = -1;

    private void Awake()
    {
        PlayerManager.Instance.PlayerInventory = this;

        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }
    }
    private void Start()
    {
        controller = PlayerManager.Instance.Player.controller;
        condition = PlayerManager.Instance.Player.condition;
        dropPosition = PlayerManager.Instance.Player.dropPosition;

        controller.throwItem += OnDropItem;
        controller.useItem += OnUseItem;
        PlayerManager.Instance.Player.addItem += AddItem;

        SelectItem(-1);
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

    // 해당 아이템이 amount 개 이상 있는지
    public bool HasItem(ItemData item, int amount) 
    {
        if (item == null || amount <= 0)
            return false;

        int total = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemdata == item)
            {
                total += slots[i].quantity;

                if (total >= amount)
                    return true;
            }
        }

        return false;
    }

    // 아이템 amount 개 소모 (재료 제거)
    public bool RemoveItem(ItemData item, int amount) 
    {
        if (!HasItem(item, amount))
            return false;

        int remaining = amount;

        for (int i = 0; i < slots.Length; i++)
        {
            ItemSlot slot = slots[i];

            if (slot.itemdata != item)
                continue;

            if (slot.quantity <= remaining)
            {
                remaining -= slot.quantity;
                slot.quantity = 0;
                slot.itemdata = null;
            }
            else
            {
                slot.quantity -= remaining;
                remaining = 0;
            }

            if (remaining <= 0)
                break;
        }

        UpdateUI();
        return true;
    }

    // 아이템 amount 개 추가(제작 결과 지급)
    public bool AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        int remaining = amount;

        // 기존 스택 채우기
        if (item.canStack)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                ItemSlot slot = slots[i];
                if (slot.itemdata == item && slot.quantity < item.maxStack)
                {
                    int space = item.maxStack - slot.quantity;
                    int add = Mathf.Min(space, remaining);

                    slot.quantity += add;
                    remaining -= add;

                    if (remaining <= 0)
                    {
                        UpdateUI();
                        return true;
                    }
                }
            }
        }
        // 빈 슬롯 채우기
        while (remaining > 0)
        {
            ItemSlot empty = GetEmptySlot();
            if (empty == null)
            {
                UpdateUI();
                return false;
            }

            int addAmount = item.canStack ? Mathf.Min(item.maxStack, remaining) : 1;

            empty.itemdata = item;
            empty.quantity = addAmount;

            remaining -= addAmount;
        }

        UpdateUI();
        return true;
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
        if (index < 0 || index >= slots.Length)
        {
            selectedItem = null;
            selectedItemIndex = -1;
            return;
        }

        ItemSlot slot = slots[index];

        if (slot.itemdata == null)
        {
            selectedItem = null;
            selectedItemIndex = -1;
            return;
        }

        selectedItem = slot.itemdata;
        selectedItemIndex = index;
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
            PlayerManager.Instance.Player.equip.UnEquip();
            selectedItem = null;
            slots[selectedItemIndex].itemdata = null;
            SelectItem(-1);
        }

        UpdateUI();
    }

    public void EquipItemInSlot(int slotIndex)
    {
        ItemSlot targetSlot = slots[slotIndex];
        ItemData itemToEquip = targetSlot.itemdata;
        Equipment playerEquipment = PlayerManager.Instance.Player.equip;

        SelectItem(slotIndex);

        foreach (var slot in slots)
        {
            if (slot.equipped)
            {
                slot.equipped = false;
            }
        }

        UpdateUI();

        if (itemToEquip == null)
        {
            playerEquipment.UnEquip(); // 플레이어 장비 해제
            return;
        }
            playerEquipment.EquipNew(itemToEquip);
            targetSlot.equipped = true;

        UpdateUI();
    }

    /// <summary>
    /// 회복(사용) 아이템 메서드
    /// </summary>
    public void OnUseItem()
    {
        if (selectedItem == null)
        {
            return;
        }

        if(selectedItem.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value); 
                        break;
                    case ConsumableType.Thirsty:
                        condition.Drink(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Stamina:
                        condition.Rest(selectedItem.consumables[i].value);
                        break;
                }
            }
            RemoveSelectedItem();
        }
    }
}
