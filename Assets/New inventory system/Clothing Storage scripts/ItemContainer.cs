using UnityEngine;

public class ItemContainer 
{
    public int rows;
    public int columns;

    public InventorySlot[] slots;
    public int TotalSlots => rows * columns;
    
    public ItemContainer(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        slots = new InventorySlot[TotalSlots];
        for (int i = 0; i < TotalSlots; i++)
            slots[i] = new InventorySlot();

    }
}
