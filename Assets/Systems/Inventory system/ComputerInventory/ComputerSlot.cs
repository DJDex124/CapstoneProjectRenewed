using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ComputerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool selected;
    public OldItemData itemInSlot;
    public int itemCount;
    public int index;

    [SerializeField] private ComputerInventory inventory;
    [SerializeField] private DragObjectUI dragUI;

    [HideInInspector] public Image slotImage;
    [HideInInspector] public Image SpriteImage;
    [HideInInspector] public TextMeshProUGUI itemCountText;

    void Awake()
    {
        slotImage = GetComponent<Image>();
        SpriteImage = transform.GetChild(0).GetComponent<Image>();
        itemCountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init()
    {
        SpriteImage.enabled = false;
        itemCountText.enabled = false;
        itemInSlot = null;
        itemCount = 0;
    }

    void Update()
    {
        float s = selected ? 1.2f : 1f;
        slotImage.transform.localScale = new Vector3(s, s, s);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dragUI.hasItem)
        {
            // Holding something: drop it into the inventory
            if (inventory.AddItem(dragUI.dragItemPrefab))
                dragUI.ClearDragItemPrefab();
        }
        else if (itemInSlot != null)
        {
            // Empty hands: pick one up from this slot
            dragUI.GetDragItemPrefab(itemInSlot);
            inventory.RemoveItem(itemInSlot);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => selected = true;
    public void OnPointerExit(PointerEventData eventData) => selected = false;
}
