using System.Collections.Generic;
using UnityEngine;



public class EndDevice : MonoBehaviour
{
    public static EndDevice current { get; private set; }
    void Awake()
    {
        current = this;
    }

    public OldItemData.ItemType acceptedType = OldItemData.ItemType.Item;
    public List<OldItemData> receivedItems = new List<OldItemData>();

    public int Quota = 3; // Number of items required to end the game
    [SerializeField] private Inventory playerInventory;

    private void Start()
    {
        GameManager.current.assignScreenCanvas();
    }

    // Call this when the player interacts with the device
    public void TryReceiveFromInventory()
    {
        if (playerInventory == null)
        {
            Debug.LogError("No inventory found!");
            return;
        }
        OldItemSlot selectedSlot = playerInventory.itemSlots[playerInventory.currentIndex];
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
        playerInventory.RemoveItem(received);
        receivedItems.Add(received);
        GameManager.current.currentQuota++;
        

        Debug.Log($"Device received: {received.itemName}");
        if (receivedItems.Count >= Quota)
        {
            GameManager.current.canStartGame = true;
        }
    }

    void endGame()
    {
        if (receivedItems.Count >= Quota)
        {
            Debug.Log("Game Ended! All required items received.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreen");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
    
}