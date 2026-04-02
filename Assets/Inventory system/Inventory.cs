using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static OldItemData;

public class OldInventory : MonoBehaviour
{
    public int currentIndex;
    int maxIndex;
    public OldItemSlot[] itemSlots;
    
    public bool isOpen;

    public static OldInventory current;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        if (itemSlots.Length == 0)
        {
            itemSlots = GetComponentsInChildren<OldItemSlot>();
        }
        foreach (var slot in itemSlots)
        {
            slot.Init();
        }
        maxIndex = itemSlots.Length;
    }

    void Update()
    {
    

        //Old system
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].index = i;
            itemSlots[i].selected = i == currentIndex;
        }

        float scroll = Mouse.current.scroll.ReadValue().y;

       
        if (scroll > 0f)
        {
            currentIndex = (currentIndex + 1) % maxIndex;
        }
        else if (scroll < 0f)
        {
            currentIndex = (currentIndex - 1 + maxIndex) % maxIndex;
        }
    }
    public void HandleInput()
    
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
        }

    }
    //Old system, will be used for hotbar and quick access slots, new system will be used for inventory management and crafting
    public void AddItem(OldItemData item)
    {

        foreach (var slot in itemSlots)
        {
            if (slot.itemInSlot == item)
            {
                slot.itemCount++;
                slot.itemCountText.text = slot.itemCount.ToString();
                return;
            }
        }
        foreach (var slot in itemSlots)
        {
            if (slot.itemInSlot == null)
            {
                slot.itemInSlot = item;
                slot.itemCount = 1;

                slot.SpriteImage.sprite = item.itemSprite;
                slot.SpriteImage.enabled = true;

                slot.itemCountText.text = "1";
                slot.itemCountText.enabled = true;


                return;
            }
        }
    }
    
    public void RemoveItem(OldItemData item)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemInSlot == item)
            {
                slot.itemCount--;

                if (slot.itemCount <= 0)
                {
                    slot.itemInSlot = null;
                    slot.itemCount = 0;
                    slot.SpriteImage.enabled = false;
                    slot.itemCountText.enabled = false;
                }
                else
                {
                    slot.itemCountText.text = slot.itemCount.ToString();
                }

                return;
            }
        }

    }
    
    public void DropSelectedItem(Transform dropOrigin)
    {
        if (currentIndex < 0 || currentIndex >= itemSlots.Length)
            return;

        OldItemSlot selectedSlot = itemSlots[currentIndex];

        if (selectedSlot.itemInSlot != null && selectedSlot.itemInSlot.itemType == OldItemData.ItemType.Item && selectedSlot.itemCount > 0)
        {
            GameObject prefab = selectedSlot.itemInSlot.pickupPrefab;

            if (prefab != null)
            {
                Vector3 dropPosition = dropOrigin.position + dropOrigin.forward + Vector3.up * 0.5f;
                GameObject droppedItem = Instantiate(prefab, dropPosition, Quaternion.identity);
                droppedItem.transform.SetParent(null);

                Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(dropOrigin.forward * 2f + Vector3.up * 1f, ForceMode.Impulse);
                }
            }
            else
            {
                Debug.LogWarning("No pickupPrefab assigned to " + selectedSlot.itemInSlot.name);
            }
            selectedSlot.itemCount--;

            if (selectedSlot.itemCount <= 0)
            {
                selectedSlot.itemInSlot = null;
                selectedSlot.itemCount = 0;
                selectedSlot.SpriteImage.enabled = false;
                selectedSlot.itemCountText.enabled = false;
            }
            else
            {
                selectedSlot.itemCountText.text = selectedSlot.itemCount.ToString();
            }
        }
        else
        {
            Debug.Log("No item to drop.");
        }
    }
    
}
