using UnityEngine;

public class CameraControllerCC : MonoBehaviour
{
    public static CameraControllerCC current;

    private float sensitivity = 2f;
    private float xRotation = 0f;

    public Ray LookRay => new Ray(transform.position, transform.forward);

    void Awake()
    {
        current = this; 
    }
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
