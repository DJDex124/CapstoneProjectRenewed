using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager current { get; private set; }

    public bool isChangingLevel = false;
    public bool mazeGenerated = false;
    public bool canStartGame = true;
    public bool noLevelsLeft = false;

    public bool isPlayerDead = false;

    [Header("Quota System")]
    public int maxQuota = 200;
    public float currentQuota;

    [Header("levelSystem")]
    public List<LevelData> levels;

    public LevelData currentLevel;

    [Header("Stats")]
    public int totalScore = 0;
    public int totalMoney = 0;
    public int totalMoneySpent = 0;
    public int totalMoneyEarned = 0;
    public int totalLootCollected = 0;
    public int totalEnemiesDefeated = 0;
    public int totalDeaths = 0;

    


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


    public void setData()
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
    public IEnumerator manualLevelChange()
    {
        if (isChangingLevel) yield break;
        isChangingLevel = true;

        Debug.Log("Level Change Initiated");
        LevelManagerCreative.current.resetLevel();
        yield return new WaitForSeconds(1f);

        setData();
        
        mazeGenerated = false;
        Debug.Log("Starting Maze Generation for Level: " + currentLevel.name);
        StartCoroutine(generateMaze());

        yield return new WaitUntil(() => mazeGenerated);
        SpawnLoot.current.findLootCells();
        EnemySystem.current.findEnemyCells();
        isChangingLevel = false;
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

    

    public void Die()
    {
        Debug.Log("Player has died!");
        //add what happens when the player dies here (e.g., respawn, game over screen, etc.)

    }


    public void endGame()
    {

        Debug.Log("Game Over!");
        //add what happens when the game ends here (e.g., show game over screen, return to main menu, etc.)
        totalScore = totalMoneyEarned + totalEnemiesDefeated + totalLootCollected - totalDeaths % 2;
    }

    public void addMoney(int amount)
    {
        totalMoney += amount;
        Debug.Log("Total Money: " + totalMoney);
        totalMoneySpent += amount;

    }
    public void removeMoney(int amount)
    {
        if (amount > totalMoney)
        {
            Debug.LogWarning("Attempted to remove more money than available. Setting total money to 0.");
            // display warning or make denying sound
            return;
        }
        totalMoney -= amount;
        if (totalMoney < 0)
        {
            totalMoney = 0;
        }
        Debug.Log("Total Money: " + totalMoney);
    }

    public void loadSpecificLevel(LevelData level)
    {
        currentLevel = level;
        setData();
        StartCoroutine(levelChange());
    }

    
}
