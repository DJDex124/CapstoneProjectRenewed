using UnityEngine;
using System.Collections;
using TMPro;

public class GameController : MonoBehaviour
{

    
    public Canvas textCanvas;
    public TextMeshProUGUI startText;
   

    public bool inRange = false;
    bool canStartGame = true;

    public GameObject elevatorPrefab;
    public float lowerDistance = 3.0f;
    public float lowerSpeed = 2.0f;
    public float lowerWaitTime = 0.6f;

    private bool isLowering = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E) && GameManager.current.canStartGame)
        {
            GameManager.current.StartGame();
            if(!isLowering)
                StartCoroutine(LowerEleWithDelay());

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.current.StartGame();
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
        isLowering = true;

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


                yield return null;
            }
        }
    }
}
