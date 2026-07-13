using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public static WeaponHandler current;
    public GameObject weapon;

    void Start()
    {
        weapon = GameObject.FindGameObjectWithTag("Weapon");
        weapon.SetActive(false);
    }

    void Update()
    {
        if (OldInventory.current != null && OldInventory.current.itemSlots[OldInventory.current.currentIndex].showTool)
        {
            spearSelected();
        }
        else
        {
            spearDeselected();
        }  
    }

    public void spearSelected()
    {
        weapon.SetActive(true);
    }
    public void spearDeselected()
    {  
       weapon.SetActive(false);  
    }
    
}
