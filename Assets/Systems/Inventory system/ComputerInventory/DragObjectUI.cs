using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DragObjectUI : MonoBehaviour
{
    public OldItemData dragItemPrefab;
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI amount;

    public bool hasItem => dragItemPrefab != null;

    void Awake()
    {
        // Let clicks pass through the drag icon to the slots underneath
        var cg = GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
        Refresh();
    }

    public void GetDragItemPrefab(OldItemData itemData)
    {
        dragItemPrefab = itemData;
        Refresh();
    }

    public void ClearDragItemPrefab()
    {
        dragItemPrefab = null;
        Refresh();
    }

    void Refresh()
    {
        if (hasItem)
        {
            itemSprite.sprite = dragItemPrefab.itemSprite;
            itemSprite.enabled = true;
            itemName.text = dragItemPrefab.itemName;
            amount.text = "1";
        }
        else
        {
            itemSprite.sprite = null;
            itemSprite.enabled = false;
            itemName.text = "";
            amount.text = "";
        }
    }
}
