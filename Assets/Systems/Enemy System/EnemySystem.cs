using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public List<GameObject> Enemy;
    public int EnemyCount;
    public int MaxEnemyCount;


    public int maxEnemyCellCount = 10;
    public int EnemyCellCount = 0;

    public List<GameObject> enemyCells;
    public List<GameObject> SpawnPoints;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            findEnemyCells();
        }
    }
    public void findEnemyCells()
    {
        enemyCells.Clear();
        GameObject[] enemyCellsArray = GameObject.FindGameObjectsWithTag("LootCell");
        foreach (GameObject cell in enemyCellsArray)
        {
            int randomIndex = Random.Range(0, 100);
            if (randomIndex < 10 && EnemyCellCount <= maxEnemyCellCount)
            {
                enemyCells.Add(cell);
                EnemyCellCount++;
            }
        }
        findLootSpawn();
        spawnLoot();
    }

    void findLootSpawn()
    {
        foreach (GameObject cell in enemyCells)
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
            if (EnemyCount < MaxEnemyCount)
            {
                int randomIndex = Random.Range(0, Enemy.Count);
                GameObject lootPrefab = Enemy[randomIndex];
                Instantiate(lootPrefab, spawnPoint.transform.position, Quaternion.identity);
                EnemyCount++;
            }
        }
    }

}
