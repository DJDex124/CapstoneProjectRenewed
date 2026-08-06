using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager current { get; private set; }
    
    public bool isChangingLevel = false;
    public bool mazeGenerated = false;
    public bool canStartGame = true;
    public bool noLevelsLeft = false;

    [Header("Quota System")]
    public int maxQuota = 200;
    public float currentQuota;

    [SerializeField]
    private TextMeshProUGUI quotaText;
    [SerializeField]
    private Canvas ScreenCanvas;

    [Header("levelSystem")]
    public List<LevelData> levels;

    public LevelData level1;
    public LevelData level2;
    public LevelData level3;
    [SerializeField]
    private LevelData currentLevel;

    public void StartGame()
    {
        StartCoroutine(levelChange());
    }
    public IEnumerator levelChange()
    {
        if (isChangingLevel) yield break;
        isChangingLevel = true;
        
        Debug.Log("Level Change Initiated");
        LevelManagerCreative.current.resetLevel();
        yield return new WaitForSeconds(1f);

        changeLevelData();
        if (noLevelsLeft)
        {

            isChangingLevel = false;
            endGame();
            yield break;
        }

        mazeGenerated = false;
        Debug.Log("Starting Maze Generation for Level: " + currentLevel.name);
        StartCoroutine(generateMaze());

        yield return new WaitUntil(() => mazeGenerated);
        SpawnLoot.current.findLootCells();
        EnemySystem.current.findEnemyCells();
        canStartGame = false;
        isChangingLevel = false;
    }
    IEnumerator generateMaze()
    {
        yield return StartCoroutine(MazeGeneration.current.StartMazeGeneration());
        mazeGenerated = true;
    }
    void changeLevelData()
    {
        if (currentLevel == null)
        {
            currentLevel = levels[0];
        }
        
        else if (levels.IndexOf(currentLevel) + 1 >= levels.Count)
        {
            Debug.Log("Game Completed!");
            
            noLevelsLeft = true;
            return;
        }
        else 
        {
            currentLevel = levels[levels.IndexOf(currentLevel) + 1];
        }

        setData();
        currentQuota = 0;
    }
   
    
    void setData()
    {
        //Maze Data
        MazeGeneration.current.maxlootCellAmount = currentLevel.lootCellCount;
        MazeGeneration.current._mazeDepth = currentLevel.mazeWidthandDepth;
        MazeGeneration.current._mazeWidth = currentLevel.mazeWidthandDepth;

        // loot data
        SpawnLoot.current.MaxLootCount = currentLevel.lootSpawnCount;
        SpawnLoot.current.maxLootCellCount = currentLevel.lootCellCount;

        // enemy data
        EnemySystem.current.MaxEnemyCount = currentLevel.enemySpawnCount;
        EnemySystem.current.maxEnemyCellCount = currentLevel.enemyCellCount;

        // quota update
        maxQuota = currentLevel.lootSpawnCount;
        EndDevice.current.Quota = maxQuota;
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
       
    }

    void Update()
    {
        
        handleScreenUI();
        
    }


    
    public void assignScreenCanvas()
    {
        ScreenCanvas = GameObject.Find("ScreenCanvas")?.GetComponent<Canvas>();
    }
     public void handleScreenUI()
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
