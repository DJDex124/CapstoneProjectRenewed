using System.Collections.Generic;
using UnityEngine;

public class SpawnLoot : MonoBehaviour
{
    public List<GameObject> Loot;
    public int LootCount;
    public int MaxLootCount;
   

    public int maxLootCellCount = 10;
    public int lootCellCount = 0;

    public List<GameObject> lootCells;
    public List<GameObject> SpawnPoints;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            findLootCells();
        }
    }
    public void findLootCells()
    {
        lootCells.Clear();
        GameObject[] lootCellsArray = GameObject.FindGameObjectsWithTag("LootCell");
        foreach (GameObject cell in lootCellsArray)
        {
            
            if ( lootCellCount <= maxLootCellCount)
            {
                lootCells.Add(cell);
                lootCellCount++;
            }
        }
        findLootSpawn();
        spawnLoot();
    }

    void findLootSpawn()
    {
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
    

