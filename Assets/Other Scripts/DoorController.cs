using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float closedPositiony = 0f;
    public float openPositiony = 5f;

    public bool doorIsOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor()
    {
        transform.position = ;
        doorIsOpen = true;
        Debug.Log("Door opened");
    }

    public void CloseDoor()
    {
        transform.position = new Vector3(transform.position.x, closedPositiony, transform.position.z);
        doorIsOpen = false;
    }
}
