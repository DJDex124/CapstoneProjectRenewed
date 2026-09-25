using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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
    bool baseLevelGenerated = false;

    public bool playeriIsDead = false;

    [Header("Quota System")]
    public int maxQuota = 200;
    public float currentQuota;

    [Header("levelSystem")]
    public List<LevelData> levels;
    public LevelData currentLevel;
    public float countDownDuration = 0f; // Duration for the countdown before maze collapse

    [Header("Stats")]
    public int totalScore = 0;
    public int totalMoney = 0;
    public int totalMoneySpent = 0;
    public int totalMoneyEarned = 0;
    public int totalLootCollected = 0;
    public int totalEnemiesDefeated = 0;
    public int totalDeaths = 0;

    [Header("Respawn References")]
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Transform respawnPosition;
    [SerializeField]
    private HealthStaminaSystem healthSystem;
    [SerializeField]
    private ScreenUISystem screenUISystem;
    [SerializeField]
    private CameraControllerCC cameraController;
    [SerializeField]
    private PlayerMovementCC playerMovement;

    public void spawnLevelBase()
    {
        if (baseLevelGenerated) return;
        baseLevelGenerated = true;

        currentLevel = levels[0];
        StartCoroutine(manualLevelChange());
        Debug.Log("Base Level Generated: " + currentLevel.name);
    }
    public void ResetGame()
    {
        Debug.Log("Resetting Game...");
        totalScore = 0;
        totalMoney = 15;
        totalMoneySpent = 0;
        totalMoneyEarned = 0;
        totalLootCollected = 0;
        totalEnemiesDefeated = 0;
        totalDeaths = 0;
        maxQuota = 200;
        currentQuota = 0;
        isChangingLevel = false;
        mazeGenerated = false;
        canStartGame = true;
        noLevelsLeft = false;
        playeriIsDead = false;
        baseLevelGenerated = false;
        if (currentLevel != null)
        {
            setData();
        }
        resetReferences();
    }
    

    public void resetReferences()
    {
        Debug.Log("Resetting References...");

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null) Debug.LogError("resetReferences: couldn't find Player!");
        }

        if (inventory == null && player != null)
        {
            inventory = GameObject.FindWithTag("Inventory").GetComponent<Inventory>();
            if (inventory == null) Debug.LogError("resetReferences: Player has no Inventory component!");
        }

        if (healthSystem == null && player != null)
        {
            healthSystem = player.GetComponentInChildren<HealthStaminaSystem>();
            if (healthSystem == null) Debug.LogError("resetReferences: Player has no HealthStaminaSystem!");
        }

        if (screenUISystem == null)
        {
            GameObject uiObj = GameObject.FindWithTag("ScreenUISystem");
            if (uiObj != null)
                screenUISystem = uiObj.GetComponentInChildren<ScreenUISystem>();
            else
                Debug.LogError("resetReferences: couldn't find ScreenUISystem!");
        }

        if (cameraController == null && player != null)
        {
            cameraController = player.GetComponentInChildren<CameraControllerCC>();
            if (cameraController == null) Debug.LogError("resetReferences: Player has no CameraControllerCC!");
        }
        if (playerMovement == null && player != null)
        {
            playerMovement = player.GetComponentInChildren<PlayerMovementCC>();
            if (playerMovement == null) Debug.LogError("resetReferences: Player has no PlayerMovementCC!");
        }

        if (respawnPosition == null)
        {
            GameObject respawnObj = GameObject.FindWithTag("RespawnPoint");
            if (respawnObj != null)
                respawnPosition = respawnObj.transform;
            else
                Debug.LogError("resetReferences: couldn't find RespawnPoint!");
        }

        if (aliveProfile == null)
        {
            GameObject volA = GameObject.FindWithTag("VolumeA");
            if (volA != null)
                aliveProfile = volA.GetComponent<Volume>();
            else
                Debug.LogError("resetReferences: couldn't find VolumeA!");
        }

        if (deadProfile == null)
        {
            GameObject volD = GameObject.FindWithTag("VolumeD");
            if (volD != null)
                deadProfile = volD.GetComponent<Volume>();
            else
                Debug.LogError("resetReferences: couldn't find VolumeD!");
        }
    }
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

        MazeGeneration.current.maxEnemyCellAmount = currentLevel.enemyCellCount;
        MazeGeneration.current.maxTrapCellAmount = currentLevel.trapCellCount;

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
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        if (inventory == null)
        {  
            inventory = gameObject.AddComponent<Inventory>();
        }
        if (healthSystem == null)
        {
            healthSystem = player.GetComponent<HealthStaminaSystem>();
        }
        
        
    }

    

    public void Die()
    {
        playeriIsDead = true;
        Debug.Log("Player has died!");
        //add what happens when the player dies here (e.g., respawn, game over screen, etc.)
        totalDeaths++;
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }
        StartCoroutine(blendVolume());
        screenUISystem.DeathScreenOn();
        playerMovement.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void respawn()
    {
        Debug.Log("respawning");
        //add respawn logic here (e.g., reset player position, health, etc.)
        playeriIsDead = false;
        
        if (inventory != null)
        inventory.resetInventory();
        RespawnPlayer();
        healthSystem.currentHealth = healthSystem.maxHealth;
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }
        resetVolume();
        screenUISystem.DeathScreenOff();
        playerMovement.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void RespawnPlayer()
    {
        if (player == null || respawnPosition == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = respawnPosition.position;
            player.transform.rotation = respawnPosition.rotation;
            cc.enabled = true;
        }
        else
        {
            player.transform.position = respawnPosition.position;
            player.transform.rotation = respawnPosition.rotation;
        }
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
        StartCoroutine(manualLevelChange());
    }
    public IEnumerator mazeCollapseCountdown(float duration)
    {
        setCountDown();
        yield return new WaitForSeconds(countDownDuration);
        //impliment indication maze is collapsing
        cameraController.triggerShake(10f, 10f);
        Debug.Log("Maze is collapsing in " + duration + " seconds!");
        yield return new WaitForSeconds(duration);
        // start the collapse of the maze 

    }
    void setCountDown()
    {
        countDownDuration = currentLevel.mazeWidthandDepth * 2f; 
    }

    [Header("Post Processing")]
    [SerializeField]
    private Volume aliveProfile;
    [SerializeField]
    private Volume deadProfile;
    IEnumerator blendVolume()
    {
        float blendDuration = 1f; // Duration of the blend
        float AtargetWeight = 0f; // Target weight for the alive profile
        float DtargetWeight = 1f; // Target weight for the dead profile
        float timeElapsed = 0f;
        while (timeElapsed < blendDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / blendDuration;
            aliveProfile.weight = Mathf.Lerp(1f, AtargetWeight, t);
            deadProfile.weight = Mathf.Lerp(0f, DtargetWeight, t);
            yield return null;

        }
    }
    void resetVolume()
    {
        aliveProfile.weight = 1f;
        deadProfile.weight = 0f;
        Debug.Log("Post-processing reset to alive profile.");
    }

}
