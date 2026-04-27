using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LootManager : MonoBehaviour
{
    public Transform LootSpawner;
    public List<GameObject> _lootPrefab;
    [Range(0, 1)]
    float spawnChance = 0.3f;
    public int spawnAmount = 10;

    [SerializeField] private MazeGeneration _mazeGeneration;   

    [Header("Spawn Settings")]
    
    public List<GameObject> enemySpawnerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Spawn(2f)); // Adjust the delay as needed
    }

    IEnumerator Spawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        spawnLoot();
        spawnEnemy();   
    }

    
    public void ClearLoot()
    {
        foreach (Transform child in LootSpawner)
        {
            Destroy(child.gameObject);
        }
    }
    public void RespawnLoot()
    {
        ClearLoot();
        spawnLoot();
    }
    public void spawnEnemy()
    {
        foreach (var cell in _mazeGeneration.enemySpawnCell)
        {
            Vector3 lootPosition = cell.GetRandomPosition(_mazeGeneration._cellSize);
            Instantiate(enemySpawnerPrefab[Random.Range(0, enemySpawnerPrefab.Count)], lootPosition, Quaternion.identity, LootSpawner);
        }
    }
    public void spawnLoot()
    {
        foreach (var cell in _mazeGeneration.lootSpawnCell)
        {
            Vector3 lootPosition = cell.GetRandomPosition(_mazeGeneration._cellSize);
            Instantiate(_lootPrefab[Random.Range(0, _lootPrefab.Count)], lootPosition, Quaternion.identity, LootSpawner);
        }
    }

}
