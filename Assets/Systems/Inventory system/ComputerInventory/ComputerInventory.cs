
using UnityEngine;

public class ComputerInventory : MonoBehaviour
{
    public int currentIndex;
    public ComputerSlot[] itemSlots;
    public bool isOpen;

    [Header("Testing only")]
    [SerializeField] private OldItemData[] debugStartingItems;

    void Start()
    {
        if (itemSlots == null || itemSlots.Length == 0)
            itemSlots = GetComponentsInChildren<ComputerSlot>();

        foreach (var slot in itemSlots)
            slot.Init();

        foreach (var item in debugStartingItems)
            AddItem(item);
    }

    public bool AddItem(OldItemData item)
    {
        // Stack onto an existing slot first
        foreach (var slot in itemSlots)
        {
            if (slot.itemInSlot == item)
            {
                slot.itemCount++;
                slot.itemCountText.text = slot.itemCount.ToString();
                return true;
            }
        }

        // Otherwise use the first empty slot
        foreach (var slot in itemSlots)
        {
            if (slot.itemInSlot == null)
            {
                slot.itemInSlot = item;
                slot.itemCount = 1;
                slot.SpriteImage.sprite = item.itemSprite;
                slot.SpriteImage.enabled = true;
                slot.itemCountText.text = "1";
                slot.itemCountText.enabled = true;
                return true;
            }
        }

        return false; // inventory full
    }

    public void RemoveItem(OldItemData item)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemInSlot == item)
            {
                slot.itemCount--;

                if (slot.itemCount <= 0)
                {
                    slot.itemInSlot = null;
                    slot.itemCount = 0;
                    slot.SpriteImage.enabled = false;
                    slot.itemCountText.enabled = false;
                }
                else
                {
                    slot.itemCountText.text = slot.itemCount.ToString();
                }
                return;
            }
        }
    }

    public void UseSelectedItem()
    {
        ComputerSlot selectedSlot = itemSlots[currentIndex];
        if (selectedSlot.itemInSlot == null)
        {
            Debug.Log("No item in selected slot.");
            return;
        }

        if (selectedSlot.itemInSlot.itemType == OldItemData.ItemType.Item)
        {
            // implement the archeology system here
        }
        else
        {
            Debug.Log("no item used");
            return;
        }

        RemoveItem(selectedSlot.itemInSlot);
    }

    public void ResetInventory()
    {
        foreach (var slot in itemSlots)
            slot.Init();
    }
}