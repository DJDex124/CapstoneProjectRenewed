using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemName = "New Item";
    public string description = "";
    public Sprite icon;
    public GameObject worldPrefab; // prefab dropped into the world

    [Header("Stacking")]
    public bool isStackable = true;
    public int maxStackSize = 64;

    [Header("Classification")]
    public ItemType itemType = ItemType.Misc;
    public ItemRarity rarity = ItemRarity.Common;

    [Header("InventoryAndClothing")]
    public bool hasStorage = false;
    public int storageRows = 0;
    public int storageColumns = 0;
    public clothingType clothingType = clothingType.NA;
}

public enum ItemType
{
    Weapon,
    clothing,
    Consumable,
    Resource,
    Misc
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum clothingType
{
    NA,
    Hat,
    shirt,
    pants,
    shoes,
    Backpack,  
}

