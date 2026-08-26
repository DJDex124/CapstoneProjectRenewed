using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class HandItemHandler : MonoBehaviour
{
   
    public GameObject weapon;
    public GameObject Torch;
    [SerializeField] private Inventory inventory;

    void Start()
    {
        weapon = GameObject.FindGameObjectWithTag("Weapon");
        weapon.SetActive(false);
        Torch.SetActive(false);
        if (inventory == null)
            inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        HandleTools();
    }

    void HandleTools()
    {
        if (inventory == null)
            return;

        if (inventory.flashLightSelected)

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
                (inventory.crowbarSelected)
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
