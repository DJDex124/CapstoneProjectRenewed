using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public static EnemySystem current { get; private set; }
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

    public List<GameObject> Enemy;
    public int EnemyCount;
    public int MaxEnemyCount;


    public int maxEnemyCellCount = 15;
    public int EnemyCellCount = 0;

    public List<GameObject> enemyCells;
    public List<GameObject> SpawnPoints;
   
        
    public void findEnemyCells()
    {
        EnemyCount = 0; 
        EnemyCellCount = 0;
        SpawnPoints.Clear();
        enemyCells.Clear();
        GameObject[] enemyCellsArray = GameObject.FindGameObjectsWithTag("EnemyCell");
        foreach (GameObject cell in enemyCellsArray)
        {
            if ( EnemyCellCount <= maxEnemyCellCount)
            {
                enemyCells.Add(cell);
                EnemyCellCount++;
            }
        }
        findEnemySpawn();
        spawnEnemy();
    }

    void findEnemySpawn()
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

    void spawnEnemy()           
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
