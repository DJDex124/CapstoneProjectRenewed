
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteractions : MonoBehaviour
{
    public GameObject Player;
    [Header("pickup settings")]
    public bool canSee;
    public float pickupRange = 3f;
    public LayerMask pickupMask;
    public LayerMask EndMask;

    public bool flCheck;
    public GameObject Flashlight;

    [SerializeField] private PlayerMovementCC playerMovement;
    public static PlayerInteractions current;

    [Header("Glowsticks")]
    [SerializeField] private GameObject glowstickPrefab;
    [SerializeField] private Transform glowstickSpawnPoint;
    [SerializeField] private int glowstickCount = 10;
    [SerializeField] private float throwForce = 6f;

    [SerializeField] private Inventory inventory;
    void Start()
    {
        current = this;
        flCheck = false;
        
        if(inventory == null) 
        inventory = GetComponent<Inventory>();
    }
    void Update()
    {
        
        handleGlowstickDrop();
        if (inventory == null)
        {
            Debug.LogError("Inventory reference is not assigned in the inspector.");
            return;
        }
        handleConsumable();
        handlePickup();
        handleDrop();
        handleEndDevice();
        if (inventory.flashLightSelected)
        {
            FlashLightToggle();
        }
    }
    public void handlePickup()
    {
        if(playerMovement == null)
        {
            Debug.LogError("Player reference is not assigned in the inspector.");
            return;
        }
        Vector3 rayOrigin = transform.position + Vector3.up * (playerMovement.controller.skinWidth + 0.05f);
        Vector3 lookDir = Camera.main.transform.forward;
        RaycastHit hit;
        canSee = Physics.Raycast(rayOrigin, lookDir, out hit, pickupRange, pickupMask);

        if (canSee && Input.GetKeyDown(KeyCode.E))
        {
            ItemPrefab itemPrefab = hit.collider.GetComponent<ItemPrefab>();
            if (itemPrefab != null)
            {
                inventory.AddItem(itemPrefab.itemData);
                Destroy(hit.collider.gameObject);
                
            }
        }
    }
    

    void handleDrop()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventory.DropSelectedItem(Player.transform);
        }
    }
    void handleEndDevice()
    {
        if (playerMovement == null)
        {
            Debug.LogError("Player reference is not assigned in the inspector.");
            return;
        }
        Vector3 rayOrigin = transform.position + Vector3.up * (playerMovement.controller.skinWidth + 0.05f);
        Vector3 lookDir = Camera.main.transform.forward;
        RaycastHit hit;
        canSee = Physics.Raycast(rayOrigin, lookDir, out hit, pickupRange, EndMask);
        if
            (canSee)
        {
            Debug.Log("Looking at end device");
        }
        if (canSee && Input.GetKeyDown(KeyCode.E))
        {
            EndDevice endDevice = hit.collider.GetComponent<EndDevice>();
            if (endDevice != null)
            {
                
                endDevice.TryReceiveFromInventory();
              
            }
        }
    }
     
    void handleConsumable()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.UseSelectedItem();
        }
        
    }
    private void OnDrawGizmos()
    {
        if (playerMovement == null)
        {
            Debug.LogError("Inventory reference is not assigned in the inspector.");
            return;
        }
        if (playerMovement == null || playerMovement.controller == null)
            return;
        if (Camera.main == null)
            return;

        Vector3 lookDir = Camera.main.transform.forward;
        Vector3 vector3 = transform.position + (Vector3.up * (playerMovement.controller.skinWidth + 0.05f));
        Vector3 rayOrigin = vector3;
        Gizmos.color = canSee ? Color.green : Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + lookDir * pickupRange);
    }
    

    void handleGlowstickDrop()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (glowstickCount <= 0)
                return;

            if (glowstickPrefab == null)
                return;

            glowstickCount--;

            GameObject glowstick = Instantiate(
                glowstickPrefab,
                glowstickSpawnPoint.position,
                Quaternion.identity
            );

            Rigidbody rb = glowstick.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 throwDirection =
                    Camera.main.transform.forward +
                    Vector3.up * 0.2f;

                rb.AddForce(
                    throwDirection.normalized * throwForce,
                    ForceMode.Impulse
                );
            }
        }
    }
    

    public void FlashLightToggle()
    {
        if (Flashlight == null)
        {
            Debug.LogError("Flashlight GameObject is not assigned in the inspector.");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (flCheck == false)
            {
                flCheck = true;
                Flashlight.SetActive(true);
            }
            else if (flCheck == true)
            {
                flCheck = false;
                Flashlight.SetActive(false);
            }
        }
    }
}
