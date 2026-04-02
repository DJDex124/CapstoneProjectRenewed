using System;

public class ClothingSlot
{
    public ItemData equippedItem;
    public ItemContainer container;

    public bool IsEmpty => equippedItem == null;

    public void Equip(ItemData item)
    {
        equippedItem = item;

        if (item != null && item.hasStorage)
            container = new ItemContainer(item.storageRows, item.storageColumns);
        else
            container = null;
    }

    public void Unequip()
    {
        equippedItem = null;
        container = null;
    }

    internal void Refresh()
    {
        throw new NotImplementedException();
    }
}
