
using UnityEngine;
using UnityEngine.InputSystem;


public class OldInventory : MonoBehaviour
{
    public int currentIndex;
    int maxIndex;
    public OldItemSlot[] itemSlots;
    public Transform handSpot;
    public GameObject heldObject;
    private int lastIndex = -1;
    public bool isOpen;

    public bool flashLightSelected = false;
    public bool crowbarSelected = false;

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

        handSpot = PlayerMovementCC.current.handSpot;
    }

    void Update()
    {
       toggleTool();


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

        if(currentIndex != lastIndex)
        {
            
            
            lastIndex = currentIndex;
        }
    }
    public void HandleInput()
    
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
        }

    }
    
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

    void UpdateHeldItem()
    {
        if (heldObject != null)
        {
            Destroy(heldObject);
        }

        OldItemSlot selectedSlot = itemSlots[currentIndex];

        if (selectedSlot.itemInSlot == null)
            return;

        GameObject heldPrefab = selectedSlot.itemInSlot.heldItem;

        if (heldPrefab != null)
        {
            heldObject = Instantiate(heldPrefab, handSpot);

            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }
    }

    public void UseSelectedItem()
    {
        HealthStaminaSystem healthSystem = HealthStaminaSystem.current;

        OldItemSlot selectedSlot = itemSlots[currentIndex];
        if (selectedSlot.itemInSlot == null)
        {
            Debug.Log("No item in selected slot.");
            return;
        }
        if (selectedSlot.itemInSlot.consumableType == OldItemData.ConsumableType.Stamina
            && healthSystem.currentStamina <= healthSystem.maxStamina)
        {
            StartCoroutine(healthSystem.disableStamina(15f));
        }
        else if (selectedSlot.itemInSlot.consumableType == OldItemData.ConsumableType.Health 
                  && healthSystem.currentHealth <= healthSystem.maxHealth )
        {
            healthSystem.healPlayer(100f);
        }
        else
        {
            return;
        }
        RemoveItem(selectedSlot.itemInSlot);
    }
    public void toggleTool()
    {
        OldItemSlot selectedSlot = itemSlots[currentIndex];
        if (selectedSlot == null)
        {
            return;
        }
        if (selectedSlot.itemInSlot != null && selectedSlot.itemInSlot.toolType == OldItemData.ToolType.Flashlight)
        {
            flashLightSelected = true;
        }
        else
        {
            flashLightSelected = false;
        }
        if (selectedSlot.itemInSlot != null && selectedSlot.itemInSlot.toolType == OldItemData.ToolType.Crowbar)
        {
            crowbarSelected = true;
        }
        else
        {
            crowbarSelected = false;
        }
    }

}