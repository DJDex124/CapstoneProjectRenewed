using UnityEngine;

public class ItemPrefab : MonoBehaviour
{

    public OldItemType itemType;
    public OldItemData itemData;

    public bool canPickup = false;   


    [Range(0f, 1f)]
    public float spawnChance = 0.3f;
    private void Update()
    {
        if (PlayerInteractions.current == null) { Debug.LogError("PlayerInteractions.current is null!"); return; }
        if (OldInventory.current == null) { Debug.LogError("OldInventory.current is null!"); return; }
        if (itemData == null) { Debug.LogError("itemData is null!"); return; }

       
    }

    
}

public enum OldItemType
{
    Stick,
    Stone,
    CompletionDevice,
    Hammer,
    Slingshot,
    Spear,
    //add more items as needed
}