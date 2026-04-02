using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ContainerSlotUI : MonoBehaviour,
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
    [SerializeField] private Image highlightOverlay;

    public int SlotIndex { get; private set; }
    private ItemContainer container;
    private InventorySlot slotData;

    private static ContainerSlotUI dragSource;
    private static GameObject dragGhost;

    public void Initialise(int index, ItemContainer itemContainer)
    {
        SlotIndex = index;
        container = itemContainer;
        Refresh();
    }

    public void Refresh()
    {
        slotData = container.slots[SlotIndex];
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // context menu can be added later
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData == null || slotData.IsEmpty) return;

        dragSource = this;

        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(InventoryUI.Instance.DragLayer, false);
        dragGhost.transform.SetAsLastSibling();

        var ghostRect = dragGhost.AddComponent<RectTransform>();
        ghostRect.sizeDelta = ((RectTransform)transform).sizeDelta;

        var ghostImage = dragGhost.AddComponent<Image>();
        ghostImage.sprite = slotData.item.icon;
        ghostImage.raycastTarget = false;

        iconImage.color = new Color(1, 1, 1, 0.4f);
        DragManager.isDraggingFromContainer = true;
        DragManager.containerSource = container;
        DragManager.containerSourceIndex = SlotIndex;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost == null) return;
        dragGhost.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragGhost);
        dragGhost = null;
        dragSource = null;
        iconImage.color = Color.white;
        DragManager.isDraggingFromContainer = false;
        DragManager.containerSource = null;
        DragManager.containerSourceIndex = -1;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragManager.isDraggingFromInventory)
        {
            int fromIndex = DragManager.inventorySourceIndex;
            InventorySlot fromSlot = InventoryManager.Instance.GetSlot(fromIndex);
            if (fromSlot == null || fromSlot.IsEmpty) return;

            container.slots[SlotIndex].item = fromSlot.item;
            container.slots[SlotIndex].quantity = fromSlot.quantity;
            InventoryManager.Instance.ClearSlot(fromIndex);
            Refresh();
        }
        else if (DragManager.isDraggingFromContainer)
        {
            if (DragManager.containerSource == container)
            {
                // swap within same container
                InventorySlot temp = new InventorySlot();
                temp.CopyFrom(container.slots[SlotIndex]);
                container.slots[SlotIndex].CopyFrom(DragManager.containerSource.slots[DragManager.containerSourceIndex]);
                DragManager.containerSource.slots[DragManager.containerSourceIndex].CopyFrom(temp);
            }
            else
            {
                // swap between different containers
                InventorySlot temp = new InventorySlot();
                temp.CopyFrom(container.slots[SlotIndex]);
                container.slots[SlotIndex].CopyFrom(DragManager.containerSource.slots[DragManager.containerSourceIndex]);
                DragManager.containerSource.slots[DragManager.containerSourceIndex].CopyFrom(temp);
            }

            dragSource?.Refresh();
            Refresh();
        }

        DragManager.isDraggingFromInventory = false;
        DragManager.isDraggingFromContainer = false;
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