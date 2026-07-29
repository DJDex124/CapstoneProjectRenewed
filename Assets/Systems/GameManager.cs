using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager current { get; private set; }
    
    public bool canSprint = true;
    public bool canJump = true;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Quota System")]
    public int maxQuota = 200;
    public float currentQuota;

    [SerializeField]
    private TextMeshProUGUI quotaText;
    [SerializeField]
    private Canvas ScreenCanvas;

    [Header("levelSystem")]
    public LevelData level1;
    public LevelData level2;
    public LevelData level3;
    [SerializeField]
    private LevelData currentLevel;

    private bool cantChangeLevel = false;

    public IEnumerator levelChange()
    {

        LevelManagerCreative.current.resetLevel();
        yield return new WaitForSeconds(1f);

        if (currentLevel == null)
        {
            currentLevel = level1;
            setData();

        }
        else if (currentLevel == level1)
        {
            currentLevel = level2;
            setData();
        }
        else if (currentLevel == level2)
        {
            currentLevel = level3;
            setData();
        }
        
        
        StartCoroutine(MazeGeneration.current.StartMazeGeneration());
    }
   

    void setData()
    {
        //Maze Data
        MazeGeneration.current.maxlootCellAmount = level1.lootCellCount;
        MazeGeneration.current._mazeDepth = currentLevel.mazeWidthandDepth;
        MazeGeneration.current._mazeWidth = currentLevel.mazeWidthandDepth;

        // loot data

    }

    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
        }
        else
        {
            current = this;
            DontDestroyOnLoad(gameObject);
        }
    }



    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (UIManager.current == null)
        {
            Debug.LogWarning("UIManager not found. Make sure it is loaded before GameManager.");
            return;
        }
        if (UIManager.current.healthSlider != null)
        {
            UIManager.current.healthSlider.minValue = 0f;
            UIManager.current.healthSlider.maxValue = maxHealth;
            UIManager.current.healthSlider.value = currentHealth;
        }

        if (UIManager.current.staminaSlider != null)
        {
            UIManager.current.staminaSlider.minValue = 0f;
            UIManager.current.staminaSlider.maxValue = maxStamina;
            UIManager.current.staminaSlider.value = currentStamina;
        }
        ScreenCanvas = GameObject.Find("ScreenCanvas")?.GetComponent<Canvas>(); 

        
    }

    void Update()
    {
        if (UIManager.current == null)
        {
            Debug.LogWarning("UIManager not found. Make sure it is loaded before GameManager.");
            return;
        }
        UIManager.current.UpdateSliders();

        if (currentHealth <= 0)
            Die();
        if (currentStamina <= 0)
            canSprint = false;
        else
            canSprint = true;
        if (currentStamina <= 10)
            canJump = false;
        else
            canJump = true;

        handleQuota();
        handleScreenUI();
    }


    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        Debug.Log("Health: " + currentHealth);
    }
    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
        
    }
    public void RegenerateStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);
        
    }

    void handleQuota()
    {

        if(currentQuota <= maxQuota )
        {
            Debug.Log("Completed Quota");
            //endgame/next level
        }
    }
    void handleScreenUI()
    {
        if (quotaText == null)
        { 
            
             if (ScreenCanvas != null)
             {
                quotaText = ScreenCanvas.GetComponentInChildren<TextMeshProUGUI>();
             }
        }
        if (quotaText != null)
        {
            quotaText.text = "Quota: " + currentQuota + "/" + maxQuota;
        }
        else
        {
            Debug.LogWarning("Quota Text component not found in the ScreenCanvas.");
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        //add what happens when the player dies here (e.g., respawn, game over screen, etc.)
    }


    void endGame()
    {
        Debug.Log("Game Over!");
        //add what happens when the game ends here (e.g., show game over screen, return to main menu, etc.)
    }


}
