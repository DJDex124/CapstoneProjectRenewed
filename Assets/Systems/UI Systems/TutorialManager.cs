using System.Collections;
using TMPro.EditorUtilities;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject inventory;

    [Header("Input References")]
    [SerializeField] private CanvasGroup WASD;
    [SerializeField] private CanvasGroup MOUSE;
    [SerializeField] private CanvasGroup INTERACT;

    [Header("Cinemachine Cam References")]
    [SerializeField] private CinemachineCamera tutorialCam;
    [SerializeField] private CinemachineCamera screenCam;
    [SerializeField] private CinemachineCamera grinderCam;

    [SerializeField] private PlayerInteractions playerInteractions;
    [SerializeField] private PlayerMovementCC playerMovement;
    [SerializeField] private CameraControllerCC cameraController;

    private bool isTutorialActive = false;
    private bool hasMouseMoved = false;
    private bool canActivateWASD = false;
    private bool hasActivatedWASD = false;
    private bool canActivateInteract = false;
    private bool hasActivatedInteract = false;

    public bool switchToScreen = false;
    public bool switchToGrinder = false;

    private void Start()
    {
        WASD.alpha = 0f;
        MOUSE.alpha = 0f;
        INTERACT.alpha = 0f;

        inventory.SetActive(false);
        playerInteractions.enabled = false;
        cameraController.enabled = false;
        playerMovement.enabled = false;
        StartCoroutine(tutorialPlay());

        resetCameras();
    }
    public void controlPanel()
    {
        if (tutorialPanel == null)
        {
            Debug.LogError("Tutorial panel is not assigned in the inspector.");
            return;
        }

        tutorialPanel.SetActive(!tutorialPanel.activeSelf);
    }
    public IEnumerator tutorialPlay()
    {
        StartCoroutine(fadeObject(MOUSE, 1f, 1f));
        yield return new WaitForSeconds(1f);
       
        isTutorialActive = true;
        cameraController.enabled = true;
        
        while (!hasMouseMoved)
        {
            yield return null;
        }

        StartCoroutine(fadeObject(MOUSE, 1f, 0f));
        yield return new WaitForSeconds(1f);
       
        StartCoroutine(fadeObject(WASD, 1f, 1f));
        yield return new WaitForSeconds(1f);
       
        canActivateWASD = true;
        playerMovement.enabled = true;
        
        while (!hasActivatedWASD)
        {
            yield return null;
        }
        StartCoroutine(fadeObject(WASD, 1f, 0f));
        yield return new WaitForSeconds(1f);
       
        StartCoroutine(fadeObject(INTERACT, 1f, 1f));
        yield return new WaitForSeconds(1f);
        
        playerInteractions.enabled = true;
        inventory.SetActive(true);
        canActivateInteract = true;

        while (!hasActivatedInteract)
        {
            yield return null;
        }
        StartCoroutine(fadeObject(INTERACT, 1f, 0f));
    }

    public IEnumerator screenAnimation()
    {
        setCam(tutorialCam);

        while (!switchToScreen)
        {
            yield return null;
        }
        setCam(screenCam);

        while (!switchToGrinder)
        {
            yield return null;
        }
        setCam(grinderCam);

        yield return new WaitForSeconds(1f);
        resetCameras();
    }
    IEnumerator fadeObject(CanvasGroup fadeTarget, float fadeDuration, float targetAlpha)
    {  
        float startAlpha = fadeTarget.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeTarget.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeTarget.alpha = targetAlpha;
    }
    void Update()
    {
        detectInteraction();

    }
    private void detectInteraction()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f && isTutorialActive)
        {
            hasMouseMoved = true;
        }
        if (canActivateWASD)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                hasActivatedWASD = true;
            }
        }

        if (canActivateInteract)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                hasActivatedInteract = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(screenAnimation());
        }
    }
    void resetCameras()
    {
        if (tutorialCam == null || screenCam == null || grinderCam == null)
        {
            Debug.LogError("One or more Cinemachine cameras are not assigned in the inspector.");
            return;
        }
        tutorialCam.Priority = 0;
        screenCam.Priority = 0;
        grinderCam.Priority = 0;
    }
    public void setCam(CinemachineCamera cam)
    {
        if (cam == null)
        {
            Debug.LogError("Cinemachine camera is not assigned in the inspector.");
            return;
        }
        resetCameras();
        cam.Priority = 15;
    }
}
