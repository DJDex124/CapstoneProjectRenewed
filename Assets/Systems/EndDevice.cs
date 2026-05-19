using System.Collections.Generic;
using UnityEngine;

public class EndDevice : MonoBehaviour
{

    public OldItemData.ItemType acceptedType = OldItemData.ItemType.Item;
    public List<OldItemData> receivedItems = new List<OldItemData>();

    public int requiredItemCount = 3; // Number of items required to end the game

    // Call this when the player interacts with the device
    public void TryReceiveFromInventory()
    {
        if (OldInventory.current == null)
        {
            Debug.LogError("No inventory found!");
            return;
        }
        OldItemSlot selectedSlot = OldInventory.current.itemSlots[OldInventory.current.currentIndex];
        if (selectedSlot.itemInSlot == null)
        {
            Debug.Log("No item in selected slot.");
            return;
        }
        if (selectedSlot.itemInSlot.itemType != acceptedType)
        {
            Debug.Log("Selected item is not the correct type.");
            return;
        }
        OldItemData received = selectedSlot.itemInSlot;
        OldInventory.current.RemoveItem(received);
        receivedItems.Add(received);

        Debug.Log($"Device received: {received.itemName}");
    }

    void endGame()
    {
        if (receivedItems.Count >= requiredItemCount)
        {
            Debug.Log("Game Ended! All required items received.");
            // Implement end game logic here (e.g., load end scene, show credits, etc.)
        }

    }
    void Update()
    {
        if (receivedItems.Count >= requiredItemCount)
        {
            endGame();
        }
    }
}