using NUnit.Framework.Interfaces;

/// <summary>
/// Runtime data representing one slot in an inventory.
/// This is pure data — no MonoBehaviour, no UI.
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public bool IsEmpty => item == null || quantity <= 0;
    public InventorySlot() { }
    

    public InventorySlot(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
    
   

    /// <summary>
    /// Returns how many of the given item can still fit in this slot.
    /// </summary>
    public int RemainingSpace()
    {
        if (IsEmpty) return int.MaxValue;
        if (item.isStackable) return item.maxStackSize - quantity;
        return 0;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    public void CopyFrom(InventorySlot other)
    {
        item = other.item;
        quantity = other.quantity;
    }
}
