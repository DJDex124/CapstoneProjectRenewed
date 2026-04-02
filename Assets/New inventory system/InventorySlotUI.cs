using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;


public class InventorySlotUI : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image highlightOverlay; // subtle hover tint
    

   

    // ----------------------------------------------------------------
    // Runtime state
    // ----------------------------------------------------------------
    public int SlotIndex { get; private set; }
    private InventorySlot slotData;

    // Static drag state shared across all slots
    private static InventorySlotUI dragSource;
    private static GameObject dragGhost; // the floating icon while dragging

    // ----------------------------------------------------------------
    // Initialise
    // ----------------------------------------------------------------

    public void Initialise(int index)
    {
        SlotIndex = index;
        // Listen to only this slot changing
        InventoryManager.Instance.OnSlotChanged.AddListener(OnSlotChanged);
        Refresh();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnSlotChanged.RemoveListener(OnSlotChanged);
    }

    private void OnSlotChanged(int changedIndex)
    {
        if (changedIndex == SlotIndex) Refresh();
    }

    // ----------------------------------------------------------------
    // Display
    // ----------------------------------------------------------------

    public void Refresh()
    {
        slotData = InventoryManager.Instance.GetSlot(SlotIndex);
        if (slotData == null || slotData.IsEmpty)
        {
            iconImage.gameObject.SetActive(false);
            quantityText.enabled = false;
            
        }
        else
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = slotData.item.icon;

            quantityText.enabled = slotData.item.isStackable && slotData.quantity > 1;
            quantityText.text = slotData.quantity.ToString();

            
        }
    }

   

    // ----------------------------------------------------------------
    // Click handling
    // ----------------------------------------------------------------

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Right click — open context menu or use item directly
            InventoryUI.Instance?.ShowContextMenu(SlotIndex, eventData.position);
        }
    }

    // ----------------------------------------------------------------
    // Drag and Drop
    // ----------------------------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData == null || slotData.IsEmpty) return;

        dragSource = this;

        // Create ghost icon that follows the cursor
        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(InventoryUI.Instance.DragLayer, false);
        dragGhost.transform.SetAsLastSibling(); // render on top

        var ghostRect = dragGhost.AddComponent<RectTransform>();
        ghostRect.sizeDelta = ((RectTransform)transform).sizeDelta;

        var ghostImage = dragGhost.AddComponent<Image>();
        ghostImage.sprite = slotData.item.icon;
        ghostImage.raycastTarget = false; // so drops hit the slot beneath

        iconImage.color = new Color(1, 1, 1, 0.4f); // fade source
        DragManager.inventorySourceIndex = SlotIndex;
        DragManager.isDraggingFromInventory = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost == null) return;
        dragGhost.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Dropped outside any valid slot — do nothing (could drop to world here)
        Destroy(dragGhost);
        dragGhost = null;
        dragSource = null;
        iconImage.color = Color.white;
        DragManager.isDraggingFromInventory = false;
        DragManager.inventorySourceIndex = -1;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragManager.isDraggingFromClothing)
        {
            ClothingSlot fomSlot = InventoryManager.Instance.GetClothingSlot(DragManager.clothingSource);
            if (fomSlot == null || fomSlot.IsEmpty) return;

            int leftover = InventoryManager.Instance.TryAddItem(fomSlot.equippedItem, 1);
            if (leftover == 0)
            {
                fomSlot.Unequip();
                InventoryUI.Instance.GetClothingSlotUI(DragManager.clothingSource)?.Refresh();
            }
        }
        if (dragSource == null || dragSource == this) return;

        bool shiftHeld = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

        if (shiftHeld)
            InventoryManager.Instance.TrySplitStack(dragSource.SlotIndex, SlotIndex);
        else
            InventoryManager.Instance.SwapSlots(dragSource.SlotIndex, SlotIndex);

        dragSource.iconImage.color = Color.white;
        Destroy(dragGhost);
        dragGhost = null;
        dragSource = null;
    }

   
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightOverlay) highlightOverlay.enabled = true;

        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightOverlay) highlightOverlay.enabled = false;
        
    }
}
