using UnityEngine;

/// <summary>
/// Attach to any world item pickup collider.
/// When the player enters the trigger (or presses interact), the item is added to inventory.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Header("Item to grant")]
    public ItemData item;
    public int quantity = 1;

    [Header("Pickup Settings")]
    [Tooltip("True = auto-pickup on trigger enter. False = requires interact key.")]
    public bool autoPickup = false;
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private float interactRange = 3;

    private bool playerInRange = false;
    private GameObject interactPrompt; // optional UI prompt, assign via Inspector

    [Header("Optional Prompt")]
    [SerializeField] private GameObject promptObject;

    private void Update()
    {
        if (!autoPickup && playerInRange && Input.GetKeyDown(interactKey))
            Pickup();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (promptObject) promptObject.SetActive(true);
        if (autoPickup) Pickup();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (promptObject) promptObject.SetActive(false);
    }

    private void Pickup()
    {
        if (InventoryManager.Instance == null) return;

        int leftover = InventoryManager.Instance.TryAddItem(item, quantity);

        if (leftover == 0)
        {
            // Everything was picked up
            if (promptObject) promptObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (leftover < quantity)
        {
            // Partially picked up — update quantity remaining on the ground
            quantity = leftover;
            Debug.Log($"Inventory full — {leftover}x {item.itemName} left on ground.");
        }
        else
        {
            Debug.Log("Inventory full — could not pick up.");
        }
    }
}
