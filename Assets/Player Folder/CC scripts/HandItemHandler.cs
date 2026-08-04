using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class HandItemHandler : MonoBehaviour
{
    public static HandItemHandler current;
    public GameObject weapon;
    public GameObject Torch;

    void Start()
    {
        weapon = GameObject.FindGameObjectWithTag("Weapon");
        weapon.SetActive(false);
        Torch.SetActive(false);
    }

    void Update()
    {
        HandleTools();
    }

    void HandleTools()
    {
        
        
            if (OldInventory.current.flashLightSelected)
            {
                torchSelected();
                Debug.Log("Torch selected");
            }
            else 
            {
                torchDeselected();
                Debug.Log("Torch deselected");
            }
            if
                (OldInventory.current.crowbarSelected)
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
    public void torchSelected()
    {
        Torch.SetActive(true);
    }
    public void torchDeselected()
    {
        Torch.SetActive(false);
    }
}
