using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class HandItemHandler : MonoBehaviour
{
    public GameObject weapon;
    public GameObject Torch;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject player;

    private bool isTorchSelected;
    private bool isSpearSelected;

    void Start()
    {
        
        if (weapon != null)
            weapon.SetActive(false);
        else
            Debug.LogWarning("HandItemHandler: no GameObject tagged 'Weapon' found.");

        if (Torch != null)
            Torch.SetActive(false);
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null) Debug.LogError("resetReferences: couldn't find Player!");
        }

        if (inventory == null && player != null)
        {
            inventory = GameObject.FindWithTag("Inventory").GetComponent<Inventory>();
            if (inventory == null) Debug.LogError("resetReferences: Player has no Inventory component!");
        }
        isTorchSelected = false;
        isSpearSelected = false;
    }

    void Update()
    {
        HandleTools();
    }

    public void HandleTools()
    {
        if (inventory == null) return;

        
        if (inventory.flashLightSelected != isTorchSelected)
        {
            isTorchSelected = inventory.flashLightSelected;
            if (Torch != null) Torch.SetActive(isTorchSelected);
            Debug.Log(isTorchSelected ? "Torch selected" : "Torch deselected");
        }

        
        if (inventory.crowbarSelected != isSpearSelected)
        {
            isSpearSelected = inventory.crowbarSelected;
            if (weapon != null) weapon.SetActive(isSpearSelected);
            Debug.Log(isSpearSelected ? "Spear selected" : "Spear deselected");
     
        }
    }
}
