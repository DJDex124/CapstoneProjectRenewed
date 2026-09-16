using Unity.Cinemachine;
using UnityEngine;

public class ComputerPlayerInteraction : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private PlayerMovementCC playerMovement;
    [SerializeField] private CameraControllerCC playerController;
    
    public void enterScreen()
    {
        if (playerCam == null || playerMovement == null || playerController == null)
        {
            Debug.LogError("One or more references are not assigned in the inspector.");
            return;
        }
        playerCam.Priority = 0;
        playerMovement.enabled = false;
        playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void exitScreen()
    {
        if (playerCam == null || playerMovement == null || playerController == null)
        {
            Debug.LogError("One or more references are not assigned in the inspector.");
            return;
        }
        playerCam.Priority = 10;
        playerMovement.enabled = true;
        playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}


