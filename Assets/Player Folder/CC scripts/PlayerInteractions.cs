using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerInteractions : MonoBehaviour
{
    public GameObject Player;
    [Header("pickup settings")]
    public bool canSee;
    public float pickupRange = 3f;
    public LayerMask pickupMask;

    public static PlayerInteractions current;

    void Start()
    {
        current = this;
    }
    void Update()
    {
        HandlePickup();
    }
    void HandlePickup()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * (PlayerMovementCC.current.controller.skinWidth + 0.05f);
        Vector3 lookDir = Camera.main.transform.forward;
        RaycastHit hit;
        canSee = Physics.Raycast(rayOrigin, lookDir, out hit, pickupRange, pickupMask);

        if (canSee)
        {
           
            Debug.Log($"Hit object: {hit.collider.gameObject.name}");
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
    void endGameLogic()
    {
        //Temporary end logic. will change so that this only handles interactions not the end of the game
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Add logic to handle the object hit by the raycast
    }
}
