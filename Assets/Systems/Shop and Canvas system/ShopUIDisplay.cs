using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Image itemImage;


    [SerializeField] private OldItemData itemData;
   
    void Start()
    {
        itemImage.sprite = itemData.itemSprite;
        itemName.text = itemData.itemName;
        itemPrice.text = ("$") + itemData.price.ToString();
    }
}
