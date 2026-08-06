using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{

    
    public Canvas textCanvas;
    public TextMeshProUGUI startText;

    public bool inRange = false;
    bool canStartGame = true;

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
}
