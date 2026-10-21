using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.ProBuilder.MeshOperations;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    public static GameController current;
    
    public Canvas textCanvas;
    public TextMeshProUGUI startText;

    
    public bool inRange = false;
    bool canStartGame = true;

    public GameObject elevatorPrefab;
    public float lowerDistance = 3.0f;
    public float lowerSpeed = 2.0f;
    public float lowerWaitTime = 0.6f;

    private bool isMoving = false;
    private bool isUp = true;
    [SerializeField]
    private GameObject elevatorCollider;

    [SerializeField]
    private Animator elevatorAnimator;

    [SerializeField] AudioSource audioSource;
    [SerializeField] private PlaySound musicSource;
    private void Awake()
    {
        current = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        startText.enabled = false;
        elevatorCollider.SetActive(false);
        audioSource = GetComponentInParent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            if ( GameManager.current.mazeGenerated)
            {
                handleElevator();
            }
            else
            {
                Debug.Log("Maze is not generated yet. Please wait.");
                //display a message to the player that no level is selected
            }


        }
        if (isMoving)
        {
            SoundManager.current.PlayLoop("Chain", audioSource);
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            SoundManager.current.StopLoop(audioSource);
        }
    }
    
    public void handleElevator()
    {
        if (!isMoving && isUp)
        {
            StartCoroutine(LowerEleWithDelay());
            if (musicSource != null)
            {
                StartCoroutine(SoundManager.current.fadeMusicOut(musicSource.audioSource));
            }
        }
        else if (!isMoving && !isUp)
        {
            StartCoroutine(LiftEleWithDelay());
            if (musicSource != null)
            {
                StartCoroutine(musicSource.musicPattern());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            startText.enabled = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            startText.enabled = false;
        }
    }

    private IEnumerator LowerEleWithDelay()
    {
        isMoving = true;
        if (elevatorAnimator != null)
        {
            elevatorAnimator.SetBool("isOpen", false);
        }
        isUp = false;
        if (elevatorCollider != null)
        {
            elevatorCollider.SetActive(true);
        }    
        yield return new WaitForSeconds(lowerWaitTime);
        if (elevatorPrefab != null)
        {

            Vector3 targetPosition = elevatorPrefab.transform.position - new Vector3(0, lowerDistance, 0);


            while (elevatorPrefab.transform.position != targetPosition)
            {
                elevatorPrefab.transform.position = Vector3.MoveTowards(
                    elevatorPrefab.transform.position,
                    targetPosition,
                    lowerSpeed * Time.deltaTime
                );


                yield return new WaitForFixedUpdate();

            }
            isMoving = false;
            if (elevatorAnimator != null)
            {
                elevatorAnimator.SetBool("isOpen", true);
            }
        }
    }
    private IEnumerator LiftEleWithDelay()
    {
        isMoving = true;
        if (elevatorAnimator != null)
        {
            elevatorAnimator.SetBool("isOpen", false);
        }

        yield return new WaitForSeconds(lowerWaitTime);
        if (elevatorPrefab != null)
        {

            Vector3 targetPosition = elevatorPrefab.transform.position + new Vector3(0, lowerDistance, 0);


            while (elevatorPrefab.transform.position != targetPosition)
            {
                elevatorPrefab.transform.position = Vector3.MoveTowards(
                    elevatorPrefab.transform.position,
                    targetPosition,
                    lowerSpeed * Time.deltaTime
                );


                yield return null;
            }
            isMoving = false;
            if (elevatorAnimator != null)
            {
                elevatorAnimator.SetBool("isOpen", true);
            }
            isUp = true;
            if (elevatorCollider != null)
            {
                elevatorCollider.SetActive(false);
            }
        }
    }
}
