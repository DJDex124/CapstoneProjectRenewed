using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject door; 
    public LevelManagerCreative levelManager;
    
    public Canvas textCanvas;
    public TextMeshProUGUI startText;

    public bool canStartGame = false;
    bool mazeHasntGenerated = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canStartGame && Input.GetKeyDown(KeyCode.E) && mazeHasntGenerated)
        {
            StartCoroutine(GameManager.current.levelChange());
            startGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            StartCoroutine(GameManager.current.levelChange());
        }

    }
    void startGame()
    {
        
        //make into a couroutine to wait for the maze to generate before disabling the door
        door.gameObject.SetActive(false);
        mazeHasntGenerated = false;
        textCanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canStartGame = true;
            startText.enabled = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canStartGame = false;
            startText.enabled = false;
        }
    }
}
