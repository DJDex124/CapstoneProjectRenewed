using TMPro;
using UnityEngine;

public class ClothingStorageSectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private RectTransform gridContainer;
    [SerializeField]
    private GameObject slotPrefab;

    private ContainerSlotUI[] slotUIs;
    private ItemContainer container;


    public void Initialise(ItemContainer itemContainer, string itemName)
    {
        container = itemContainer;
        headerText.text = itemName;

        slotUIs = new ContainerSlotUI[container.TotalSlots];

        for (int i = 0; i < container.TotalSlots; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, gridContainer);
            slotGO.name = $"ContainerSlot_{i}";

            ContainerSlotUI slotUI = slotGO.GetComponent<ContainerSlotUI>();
            slotUI.Initialise(i, container);
            slotUIs[i] = slotUI;
        }
    }
}
