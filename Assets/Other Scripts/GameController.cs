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
            startGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            levelManager.resetLevel();
        }

    }
    void startGame()
    {
        levelManager.generateMaze();
        door.gameObject.SetActive(false);
        mazeHasntGenerated = false;
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
