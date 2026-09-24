using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenUISystem : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField]
    private Canvas screenCanvas;
    [SerializeField]
    private Canvas storeCanvas;
    [SerializeField]
    private Canvas levelsCanvas;
    [SerializeField]
    private Canvas PauseMenuCanvas;
    [SerializeField]
    private Canvas deathScreeenCanvas;
    [SerializeField]
    private Canvas scoreCanvas;

    [Header("Level System")]
    [SerializeField]
    private LevelData[] levels;
    [SerializeField]
    private TextMeshProUGUI currentLevel;

    [Header("Screen System")]
    [SerializeField]
    private TextMeshProUGUI currentMoney;
    [SerializeField]
    private ItemShopSpawner itemSpawner;

    [Header("Player Reference")]
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private HealthStaminaSystem playerStats;
    [SerializeField]
    private PlayerMovementCC playerMovement;

    [Header("Score")]
    [SerializeField]
    private TextMeshProUGUI Score;
    [SerializeField]
    private TextMeshProUGUI MoneyEarned;
    [SerializeField]
    private TextMeshProUGUI Deaths;
    [SerializeField]
    private TextMeshProUGUI EnemiesDefeated;

    private bool level1Generated = false;
    private bool level2Generated = false;
    private bool level3Generated = false;

    public bool canPause = true;


    private void Awake()
    {
        if (screenCanvas != null || storeCanvas != null || levelsCanvas != null )
        {
            screenCanvas.enabled = true;
            storeCanvas.enabled = false;
            levelsCanvas.enabled = false;
        }
        if (PauseMenuCanvas != null)
        {
            PauseMenuCanvas.enabled = false;
        }
        if (itemSpawner == null)
        {
            //itemSpawner = GameObject.FindWithTag("EndDevice");
        }
        if (deathScreeenCanvas == null)
        {
            deathScreeenCanvas = GameObject.FindWithTag("DeathScreen").GetComponent<Canvas>();
        }
        if (deathScreeenCanvas != null)
        {
            deathScreeenCanvas.enabled = false;
        }
        player = GameObject.FindWithTag("Player");
        if (player != null && playerStats == null)
        {
            playerStats = player.GetComponent<HealthStaminaSystem>();
        }
        else
        {
            Debug.LogWarning("Player GameObject not found. Make sure it is tagged as 'Player'.");
        }
        scoreCanvas.enabled = false;
        if (player != null && playerMovement == null)
        {
            playerMovement = player.GetComponent<PlayerMovementCC>();
        }
    }

    void Update()
    {
        handleScreenUI();
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            if (GameManager.current.playeriIsDead)
            {
                return; 
            }
            togglePauseGame();
        }
       
    }
    //choose levels function

    public void chooseLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            LevelData selectedLevel = levels[levelIndex];
            GameManager.current.currentLevel = selectedLevel;
            currentLevel.text = "Current Level: " + selectedLevel.name;
        }
        else
        {
            Debug.LogWarning("Invalid level index: " + levelIndex);
        }
        if (level1Generated == true && levelIndex == 0)
        {
            Debug.LogWarning("Level has already been generated. Please select a different level.");
            return;
        }
        if (level2Generated == true && levelIndex == 1)
        {
            Debug.LogWarning("Level has already been generated. Please select a different level.");
            return;
        }
        if (level3Generated == true && levelIndex == 3)
        {
            Debug.LogWarning("Level has already been generated. Please select a different level.");
            return;
        }

        if (GameManager.current.currentLevel != null)
        {
            if (GameManager.current.currentLevel.levelPrice > GameManager.current.totalMoney)
            {
                Debug.LogWarning("Not enough money to select this level. Please select a different level.");
                return;
            }
            StartCoroutine(GameManager.current.manualLevelChange());
            LevelData selectedLevel = levels[levelIndex];
            GameManager.current.addMoney(-selectedLevel.levelPrice);
            
        }
        else 
        {
            if (level3Generated == true )
            {
                Debug.LogWarning("Level has already been generated. Please select a different level.");
                GameManager.current.endGame();
                return;
            }
            Debug.LogWarning("No level selected. Please select a level before starting the game.");
        }
        if (levelIndex == 0)
        {
            level1Generated = true;
        }
        if (levelIndex == 1)
        {
            level2Generated = true;
        }
        if (levelIndex == 2)
        {
            level3Generated = true;
        }

    }



    public void handleScreenUI()
    {

        if (currentMoney != null)
        {
            currentMoney.text = "Money: " + GameManager.current.totalMoney;
        }
        else
        {
            Debug.LogWarning("Quota Text component not found in the ScreenCanvas.");
        }
    }

    public void togglePauseGame()
    {
        if (PauseMenuCanvas != null)
        {
            if (PauseMenuCanvas.enabled)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    void handleScoreScreen()
    {
        if (Score != null)
        {
            Score.text = "Score: " + GameManager.current.totalScore;
        }
        if (MoneyEarned != null)
        {
            MoneyEarned.text = "Money Earned: " + GameManager.current.totalMoneySpent;
        }
        if (Deaths != null)
        {
            Deaths.text = "Deaths: " + GameManager.current.totalDeaths;
        }
        if (EnemiesDefeated != null)
        {
            EnemiesDefeated.text = "Enemies Defeated: " + GameManager.current.totalEnemiesDefeated;
        }
    }

    public void DeathScreenOn()
    {
        Debug.Log("Death Screen Displayed");
        deathScreeenCanvas.enabled = true;
        handleScoreScreen();
        canPause = false;
    }
    public void DeathScreenOff()
    {
        Debug.Log("Death Screen Disabled");
        deathScreeenCanvas.enabled = false;
        canPause = true;
    }

    #region button functions
    public void back()
    {
        screenCanvas.enabled = true;
        storeCanvas.enabled = false;
        levelsCanvas.enabled = false;
    }
    public void toStore()
    {
        screenCanvas.enabled = false;
        storeCanvas.enabled = true;
        levelsCanvas.enabled = false;
    }
    public void toLevels()
    {
        screenCanvas.enabled = false;
        storeCanvas.enabled = false;
        levelsCanvas.enabled = true;
    }
    public void PauseGame()
    {
        PauseMenuCanvas.enabled = true;
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CameraControllerCC.current.paused = true;
        playerMovement.enabled = false;
    }
    public void ResumeGame()
    {
        PauseMenuCanvas.enabled = false;
        Time.timeScale = 1f; // Resume the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CameraControllerCC.current.paused = false;
        playerMovement.enabled = true;

    }
    public void startButton()
    {
        SceneManager.LoadScene("Gameplay Display");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.current.resetReferences();
    }
    public void quitButton()
    {
        Application.Quit();
    }
    public void mainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void openScore()
    {
        if (scoreCanvas != null)
        {
            if (scoreCanvas.enabled)
            {
                scoreCanvas.enabled = false;
                deathScreeenCanvas.enabled = true;
            }
            else
            {
                deathScreeenCanvas.enabled = false;
                scoreCanvas.enabled = true;
            }
            
        }
    }
    public void endGame()
    {
        GameManager.current.endGame();
    }
    public void respawn()
    {
        GameManager.current.respawn();

    }
    public void clickSound()
    {
        //AudioManager.current.playSound("ButtonClick");
    }
    public void handleBuy(OldItemData item)
    {
        if (GameManager.current.totalMoney >= item.price)
        {
            GameManager.current.addMoney(-item.price);
            //buy item logic 
            itemSpawner.SpawnItem(item);
            Debug.Log("Bought item: " + item.itemName);
        }
        else
        {
            Debug.Log("Not enough money to buy: " + item.itemName);
        }

    }

    #endregion
}
