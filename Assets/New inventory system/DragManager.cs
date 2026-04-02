public static class DragManager
{
    public static int inventorySourceIndex = -1;
    public static clothingType clothingSource = clothingType.NA;
   
    public static bool isDraggingFromInventory = false;
    public static bool isDraggingFromClothing = false;

    public static bool isDraggingFromContainer = false;
    public static ItemContainer containerSource = null;
    public static int containerSourceIndex = -1;
}
