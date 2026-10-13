using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private List<MaggotEnemy> currentEnemies;

    
    public void findEnemyCells()
    {
        EnemyCount = 0;
        EnemyCellCount = 0;
        SpawnPoints.Clear();
        enemyCells.Clear();
        GameObject[] enemyCellsArray = GameObject.FindGameObjectsWithTag("EnemyCell");
        foreach (GameObject cell in enemyCellsArray)
        {
            if (EnemyCellCount <= maxEnemyCellCount)
            {
                enemyCells.Add(cell);
                EnemyCellCount++;
            }
        }
        findEnemySpawn();
        
    }

    void findEnemySpawn()
    {
        foreach (GameObject cell in enemyCells)
        {
            Instantiate(Enemy[Random.Range(0, Enemy.Count)], cell.transform.position, Quaternion.identity);
        }
    }

    

    public void removeEnemies()
    {
        GameObject[] spawnedEnemies = GameObject.FindGameObjectsWithTag("Enemy");


        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
        currentEnemies.Clear();
    }
}
   
