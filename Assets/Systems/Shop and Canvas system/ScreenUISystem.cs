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
    private GameObject itemSpawner;

    [Header("Player Reference")]
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private HealthStaminaSystem playerStats;

    [Header("Score")]
    [SerializeField]
    private TextMeshProUGUI Score;
    [SerializeField]
    private TextMeshProUGUI MoneyEarned;
    [SerializeField]
    private TextMeshProUGUI Deaths;
    [SerializeField]
    private TextMeshProUGUI EnemiesDefeated;




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
            itemSpawner = GameObject.FindWithTag("EndDevice");
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
        if (player != null)
        {
            playerStats = player.GetComponent<HealthStaminaSystem>();
        }
        else
        {
            Debug.LogWarning("Player GameObject not found. Make sure it is tagged as 'Player'.");
        }
        scoreCanvas.enabled = false;
    }

    void Update()
    {
        handleScreenUI();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            togglePauseGame();
        }
        handleDeathScreen();
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
        if (GameManager.current.currentLevel != null)
        {
            StartCoroutine(GameManager.current.manualLevelChange());
        }
        else
        {
            Debug.LogWarning("No level selected. Please select a level before starting the game.");
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

    void handleDeathScreen()
    {
        if ( playerStats.currentHealth <= 0)
        {
            deathScreeenCanvas.enabled = true;
            Time.timeScale = 0f; // Pause the game
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameManager.current.endGame();
            handleScoreScreen();

        }
        else
        {
            deathScreeenCanvas.enabled = false;
        }
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
    }
    public void ResumeGame()
    {
        PauseMenuCanvas.enabled = false;
        Time.timeScale = 1f; // Resume the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CameraControllerCC.current.paused = false;

    }
    public void startButton()
    {
        SceneManager.LoadScene("Gameplay Display");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            itemSpawner.GetComponent<ItemShopSpawner>().SpawnItem(item);
            Debug.Log("Bought item: " + item.itemName);
        }
        else
        {
            Debug.Log("Not enough money to buy: " + item.itemName);
        }

    }
    #endregion
}
