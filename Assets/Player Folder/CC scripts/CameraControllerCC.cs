using UnityEngine;

public class CameraControllerCC : MonoBehaviour
{
    public static CameraControllerCC current;

    private float sensitivity = 2f;
    private float xRotation = 0f;

    public bool paused = false;
    public Ray LookRay => new Ray(transform.position, transform.forward);

    [Header("Camera Shake Settings")]
    private Vector3 originalPos;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.3f;
    private float dampingSpeed = 1.0f;

    void Awake()
    {
        current = this; 
    }
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalPos = transform.localPosition;
    }

    
    void Update()
    {
        if (!paused)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.parent.Rotate(Vector3.up * mouseX);
        }
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }
    public void triggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
