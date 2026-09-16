using System.Collections.Generic;
using UnityEngine;

public class SpawnLoot : MonoBehaviour
{
    public static SpawnLoot current { get; private set; }
    private void Awake()
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


    public List<GameObject> Loot;
    public int LootCount;
    public int MaxLootCount;
   

    public int maxLootCellCount = 10;
    public int lootCellCount = 0;

    public List<GameObject> lootCells;
    public List<GameObject> SpawnPoints;
    
    public void findLootCells()
    {
        LootCount = 0;
        lootCellCount = 0;
        lootCells.Clear();
        SpawnPoints.Clear();
        GameObject[] lootCellsArray = GameObject.FindGameObjectsWithTag("LootCell");
        foreach (GameObject cell in lootCellsArray)
        {
            
            if ( lootCellCount <= maxLootCellCount)
            {
                lootCells.Add(cell);
                lootCellCount++;
            }
        }

        spawnRandomLoot();
    }

    void findLootSpawn()
    {
        // use this if I want to make sure that loot spawns in each loot cell and not randomly everywhere
        // downside is that if there are more loot cells than loot to spawn, some loot cells will be empty
        foreach (GameObject cell in lootCells)
        {
            List<GameObject> potentialSpawns = new List<GameObject>();

            foreach (Transform child in cell.transform)
            {
                if (child.CompareTag("SpawnPoint"))
                    potentialSpawns.Add(child.gameObject);
            }
            if (potentialSpawns.Count > 0)
            {
                int randomIndex = Random.Range(0, potentialSpawns.Count);
                GameObject spawnPosition = potentialSpawns[randomIndex];

                potentialSpawns.RemoveAt(randomIndex);
                SpawnPoints.Add(spawnPosition);
            }
        }
    }
   
    void spawnRandomLoot()
    {
        // use this if I want spawns to be randomly everywhere and if I want more loot to spawn than loot cells
        foreach (GameObject cell in lootCells)
        {
            foreach (Transform child in cell.transform)
            {
                if (child.CompareTag("SpawnPoint"))
                    SpawnPoints.Add(child.gameObject);
            }
        }
        for (int i = 0; i < MaxLootCount; i++)
        { 
            Debug.Log("Spawning Loot: " + i);
            int randomIndex = Random.Range(0, SpawnPoints.Count);
            GameObject spawnPosition = SpawnPoints[randomIndex];
            SpawnPoints.RemoveAt(randomIndex);

            int randomLootIndex = Random.Range(0, Loot.Count);
            GameObject lootPrefab = Loot[randomLootIndex];
            Instantiate(lootPrefab, spawnPosition.transform.position, Quaternion.identity);
            LootCount++;

        }
    }
    void spawnLoot()
    {
        foreach (GameObject spawnPoint in SpawnPoints)
        {
            if (LootCount < MaxLootCount)
            {
                int randomIndex = Random.Range(0, Loot.Count);
                GameObject lootPrefab = Loot[randomIndex];
                Instantiate(lootPrefab, spawnPoint.transform.position, Quaternion.identity);
                LootCount++;
            }
        }
    }

}
    

