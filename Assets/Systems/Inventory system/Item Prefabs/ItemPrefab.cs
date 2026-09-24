using ITISKIRUHERE;
using UnityEngine;

public class ItemPrefab : MonoBehaviour
{
    public OldItemData itemData;
    public bool canPickup = false;
    

    private AdvancedOutline outline;
    private void Start()
    {
        outline = GetComponent<AdvancedOutline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            outline.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            outline.enabled = false;
        }
    }
}

