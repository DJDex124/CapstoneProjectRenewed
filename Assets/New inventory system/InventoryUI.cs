using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;


public class InventoryUI : MonoBehaviour
{
   
    public static InventoryUI Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private RectTransform gridContainer;
    

    [Header("Drag")]
    public RectTransform DragLayer; // ghost icon is parented here

    [Header("Slot Prefab")]
    [SerializeField] private GameObject slotPrefab;

    [Header("Clothing Storage")]
    [SerializeField] private RectTransform clothingStorageList;
    [SerializeField] private GameObject clothingStorageSectionPrefab;

    [Header("Context Menu")]
    [SerializeField] private GameObject contextMenuPanel;
    [SerializeField] private Button contextUseButton;
    [SerializeField] private Button contextDropButton;
    [SerializeField] private Button contextSplitButton;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    [Header("References")]
    [SerializeField] private Camera playerCamera;

    // ----------------------------------------------------------------
    private InventoryManager manager;
    private InventorySlotUI[] slotUIs;
    private int contextMenuSlotIndex = -1;
    private Dictionary<clothingType, ClothingStorageSectionUI> activeSections
    = new Dictionary<clothingType, ClothingStorageSectionUI>();

    // ----------------------------------------------------------------
    // Lifecycle
    // ----------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {

        manager = InventoryManager.Instance;

        manager.OnInventoryOpened.AddListener(OnInventoryOpened);
        manager.OnInventoryClosed.AddListener(OnInventoryClose);

        BuildGrid();
        inventoryPanel.SetActive(false);
        HideContextMenu();
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
            manager.ToggleInventory();

        if (Keyboard.current.escapeKey.wasPressedThisFrame && manager.IsOpen)
            manager.CloseInventory();
    }

    // ----------------------------------------------------------------
    // Grid Construction
    // ----------------------------------------------------------------

    private void BuildGrid()
    {
        slotUIs = new InventorySlotUI[manager.TotalSlots];

        for (int i = 0; i < manager.TotalSlots; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, gridContainer);
            slotGO.name = $"Slot_{i}";

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUI.Initialise(i);
            slotUIs[i] = slotUI;
        }
    }

    // ----------------------------------------------------------------
    // Open / Close
    // ----------------------------------------------------------------

    private void OnInventoryOpened()
    {
        inventoryPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnInventoryClose()
    {
        inventoryPanel.SetActive(false);
        HideContextMenu();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ----------------------------------------------------------------
    // Tooltip
    // ----------------------------------------------------------------

   

    // ----------------------------------------------------------------
    // Context Menu
    // ----------------------------------------------------------------

    public void ShowContextMenu(int slotIndex, Vector2 screenPosition)
    {
        InventorySlot slot = manager.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty) return;

        contextMenuSlotIndex = slotIndex;
        contextMenuPanel.SetActive(true);
        contextMenuPanel.transform.position = ClampToScreen(screenPosition);

        // Configure buttons
        contextUseButton.onClick.RemoveAllListeners();
        contextUseButton.onClick.AddListener(() =>
        {
            UseItem(slotIndex);
            HideContextMenu();
        });

        contextDropButton.onClick.RemoveAllListeners();
        contextDropButton.onClick.AddListener(() =>
        {
            DropItem(slotIndex);
            HideContextMenu();
        });

        contextSplitButton.gameObject.SetActive(slot.item.isStackable && slot.quantity > 1);
        contextSplitButton.onClick.RemoveAllListeners();
        contextSplitButton.onClick.AddListener(() =>
        {
            // Split into first available empty slot
            for (int i = 0; i < manager.TotalSlots; i++)
            {
                if (manager.GetSlot(i).IsEmpty)
                {
                    manager.TrySplitStack(slotIndex, i);
                    break;
                }
            }
            HideContextMenu();
        });
    }

    public void HideContextMenu()
    {
        if (contextMenuPanel) contextMenuPanel.SetActive(false);
        contextMenuSlotIndex = -1;
    }

    // ----------------------------------------------------------------
    // Item Actions
    // ----------------------------------------------------------------

    private void UseItem(int slotIndex)
    {
        InventorySlot slot = manager.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty) return;

        // Extend this with your own use logic per item type
        switch (slot.item.itemType)
        {
            case ItemType.Consumable:
                Debug.Log($"Using consumable: {slot.item.itemName}");
                manager.TryRemoveItem(slot.item, 1);
                break;
            default:
                Debug.Log($"Equipping / using: {slot.item.itemName}");
                break;
        }
    }

    private void DropItem(int slotIndex)
    {
        InventorySlot slot = manager.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty) return;

        // Spawn world prefab in front of the player
        if (slot.item.worldPrefab != null)
        {
            Transform cam = Camera.main?.transform;
            Vector3 spawnPos = cam != null
                ? cam.position + cam.forward * 1.5f
                : Vector3.zero;
            Instantiate(slot.item.worldPrefab, spawnPos, Quaternion.identity);
        }

        string itemName = slot.item.itemName;
        InventoryManager.Instance.ClearSlot(slotIndex);
        Debug.Log($"Dropped: {itemName}");
    }

    // ----------------------------------------------------------------
    // Clothing Slots
    // ----------------------------------------------------------------
    public void ShowContextMenu(clothingType slotType, Vector2 screenPosition)
    {
        ClothingSlot slot = InventoryManager.Instance.GetClothingSlot(slotType);
        if (slot == null || slot.IsEmpty) return;

        contextMenuPanel.SetActive(true);
        contextMenuPanel.transform.position = ClampToScreen(screenPosition);

        contextUseButton.onClick.RemoveAllListeners();
        contextUseButton.onClick.AddListener(() =>
        {
            HideContextMenu();
        });

        contextDropButton.onClick.RemoveAllListeners();
        contextDropButton.onClick.AddListener(() =>
        {
            InventoryManager.Instance.UnequipItem(slotType);
            HideContextMenu();
        });

        contextSplitButton.gameObject.SetActive(false);
    }
    public ClothingSlotUI GetClothingSlotUI(clothingType type)
    {
        foreach (ClothingSlotUI slot in FindObjectsByType<ClothingSlotUI>(FindObjectsSortMode.None))
            if (slot.slotType == type) return slot;
        return null;
    }

    public void SpawnClothingStorage(clothingType type, ItemContainer container, string itemName)
    {
        if (activeSections.ContainsKey(type)) return;

        GameObject sectionGO = Instantiate(clothingStorageSectionPrefab, clothingStorageList);
        ClothingStorageSectionUI section = sectionGO.GetComponent<ClothingStorageSectionUI>();
        section.Initialise(container, itemName);
        activeSections[type] = section;
    }

    public void DestroyClothingStorage(clothingType type)
    {
        if (!activeSections.ContainsKey(type)) return;

        Destroy(activeSections[type].gameObject);
        activeSections.Remove(type);
    }

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    private Vector3 ClampToScreen(Vector3 pos)
    {
        float w = Screen.width;
        float h = Screen.height;
        pos.x = Mathf.Clamp(pos.x, 0, w - 200);
        pos.y = Mathf.Clamp(pos.y, 100, h);
        return pos;
    }
    
}
