using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Progress;

public class PlayerInteractions : MonoBehaviour
{
    public GameObject Player;
    [Header("pickup settings")]
    public bool canSee;
    public float pickupRange = 3f;
    public LayerMask pickupMask;
    public LayerMask EndMask;
    


    public static PlayerInteractions current;

    [Header("Glowsticks")]
    [SerializeField] private GameObject glowstickPrefab;
    [SerializeField] private Transform glowstickSpawnPoint;
    [SerializeField] private int glowstickCount = 10;
    [SerializeField] private float throwForce = 6f;

    void Start()
    {
        current = this;
    }
    void Update()
    {
        HandlePickup();
        handleDrop();
        HandleGlowstickDrop();
        HandleEndDevice();
    }
    public void HandlePickup()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * (PlayerMovementCC.current.controller.skinWidth + 0.05f);
        Vector3 lookDir = Camera.main.transform.forward;
        RaycastHit hit;
        canSee = Physics.Raycast(rayOrigin, lookDir, out hit, pickupRange, pickupMask);

        if (canSee && Input.GetKeyDown(KeyCode.E))
        {
            ItemPrefab itemPrefab = hit.collider.GetComponent<ItemPrefab>();
            if (itemPrefab != null)
            {
                OldInventory.current.AddItem(itemPrefab.itemData);
                Destroy(hit.collider.gameObject);
                
            }
        }
    }

    void handleDrop()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OldInventory.current.DropSelectedItem(Player.transform);
        }
    }
    void HandleEndDevice()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * (PlayerMovementCC.current.controller.skinWidth + 0.05f);
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
     
    private void OnDrawGizmos()
    {
        if (PlayerMovementCC.current == null || PlayerMovementCC.current.controller == null)
            return;
        if (Camera.main == null)
            return;

        Vector3 lookDir = Camera.main.transform.forward;
        Vector3 vector3 = transform.position + (Vector3.up * (PlayerMovementCC.current.controller.skinWidth + 0.05f));
        Vector3 rayOrigin = vector3;
        Gizmos.color = canSee ? Color.green : Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + lookDir * pickupRange);
    }
    

    void HandleGlowstickDrop()
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
}
