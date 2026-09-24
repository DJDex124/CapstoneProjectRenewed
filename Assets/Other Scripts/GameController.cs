using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.ProBuilder.MeshOperations;

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

    private void Awake()
    {
        current = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startText.enabled = false;
        elevatorCollider.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
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
        
    }
    
    public void handleElevator()
    {
        if (!isMoving && isUp)
        {
            StartCoroutine(LowerEleWithDelay());
        }
        else if (!isMoving && !isUp)
        {
            StartCoroutine(LiftEleWithDelay());
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
