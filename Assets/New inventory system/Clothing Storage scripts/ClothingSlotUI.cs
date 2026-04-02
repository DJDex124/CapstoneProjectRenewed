using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ClothingSlotUI : MonoBehaviour,

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
    [SerializeField] private Image highlightOverlay; 

    public clothingType slotType;
    private ClothingSlot clothingSlot;

    private static ClothingSlotUI dragSource;
    private static GameObject dragGhost;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        clothingSlot = InventoryManager.Instance.GetClothingSlot(slotType);
        if (clothingSlot == null || clothingSlot.IsEmpty)
            iconImage.gameObject.SetActive(false);
        else
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = clothingSlot.equippedItem.icon;
        }
    }



    // ----------------------------------------------------------------
    // Click handling
    // ----------------------------------------------------------------

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            InventoryUI.Instance?.ShowContextMenu(slotType, eventData.position);
    }

    // ----------------------------------------------------------------
    // Drag and Drop
    // ----------------------------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (clothingSlot == null || clothingSlot.IsEmpty) return;

        dragSource = this;

        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(InventoryUI.Instance.DragLayer, false);
        dragGhost.transform.SetAsLastSibling();

        var ghostRect = dragGhost.AddComponent<RectTransform>();
        ghostRect.sizeDelta = ((RectTransform)transform).sizeDelta;

        var ghostImage = dragGhost.AddComponent<Image>();
        ghostImage.sprite = clothingSlot.equippedItem.icon;
        ghostImage.raycastTarget = false;

        iconImage.color = new Color(1, 1, 1, 0.4f);
        DragManager.isDraggingFromClothing = true;
        DragManager.clothingSource = slotType;
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
        DragManager.isDraggingFromClothing = false;
        DragManager.clothingSource = clothingType.NA;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragManager.isDraggingFromInventory)
        {
            int fromIndex = DragManager.inventorySourceIndex;
            InventorySlot fromSlot = InventoryManager.Instance.GetSlot(fromIndex);

            if (fromSlot == null || fromSlot.IsEmpty) return;

            // Check the item matches this clothing slot type
            if (fromSlot.item.clothingType != slotType) return;

            // Equip it
            InventoryManager.Instance.EquipItem(slotType, fromSlot.item);
            InventoryManager.Instance.ClearSlot(fromIndex);
            Refresh();
        }
        else if (DragManager.isDraggingFromClothing)
        {
            ClothingSlot fromSlot = InventoryManager.Instance.GetClothingSlot(DragManager.clothingSource);
            if (fromSlot == null || fromSlot.IsEmpty) return;

            if (fromSlot.equippedItem.clothingType != slotType) return; 

            InventoryUI.Instance.GetClothingSlotUI(DragManager.clothingSource)?.Refresh();
            InventoryManager.Instance.SwapClothingSlots(DragManager.clothingSource, slotType);
            Refresh(); 
        }

        DragManager.isDraggingFromInventory = false;
        DragManager.isDraggingFromClothing = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightOverlay) highlightOverlay.gameObject.SetActive(true);


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightOverlay) highlightOverlay.gameObject.SetActive(false);

    }
}

