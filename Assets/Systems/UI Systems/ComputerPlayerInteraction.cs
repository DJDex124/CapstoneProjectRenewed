using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ComputerPlayerInteraction : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private PlayerMovementCC playerMovement;
    [SerializeField] private CameraControllerCC playerController;
    [SerializeField] private Canvas promptCanvas;
    [SerializeField] private TextMeshProUGUI promptText;
    private bool isPlayerInRange = false;
    [SerializeField] private ScreenUISystem UISystem;
    private bool isInScreen = false;

    private void Update()
    {
        if (GameManager.current.playeriIsDead)
        {
            return;
        }
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            enterScreen();
            isInScreen = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isInScreen)
        {
            exitScreen();
            isInScreen = false;
        }
    }
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
        UISystem.canPause = false;
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
        UISystem.canPause = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered the interaction range.");
            if (promptText == null)
            {
                Debug.LogError("Prompt Text reference is not assigned in the inspector.");
                return;
            }
            promptText.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited the interaction range.");
            if (promptText == null)
            {
                Debug.LogError("Prompt Text reference is not assigned in the inspector.");
                return;
            }
            promptText.enabled = false;
        }
    }

}


