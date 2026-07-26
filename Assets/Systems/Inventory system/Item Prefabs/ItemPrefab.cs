using UnityEngine;

public class ItemPrefab : MonoBehaviour
{
    public OldItemData itemData;
    public bool canPickup = false;   

    private void Update()
    {
        if (PlayerInteractions.current == null) { Debug.LogError("PlayerInteractions.current is null!"); return; }
        if (OldInventory.current == null) { Debug.LogError("OldInventory.current is null!"); return; }
        if (itemData == null) { Debug.LogError("itemData is null!"); return; }

       
    } 
}

