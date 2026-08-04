using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/OldItem")]
public class OldItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject pickupPrefab;
    public GameObject heldItem;
    public ItemType itemType;
    public ConsumableType consumableType;
    public ToolType toolType;
    public GameObject clubHolder;

    public int itemValue;   
    public int maxStackSize;

    

    public enum ItemType
    {
        Consumable,
        Item,
        Tool,
        DontUse,
        endDevice
    }
    public enum ConsumableType
    {
        NA,
        Health,
        Stamina
        
    }
    public enum ToolType
    {
        NA,
        Flashlight,
        Glowstick,
        Crowbar
    }
}
