using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LootManager : MonoBehaviour
{
    public Transform LootSpawner;
    public List<GameObject> _lootPrefab;
    [Range(0, 1)]
    float spawnChance = 0.3f;

    [SerializeField]
    private MazeGeneration _mazeGeneration;

    [Header("Spawn Settings")]
    
    public List<GameObject> enemySpawnerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnLootWithDelay(2f)); // Adjust the delay as needed
    }

    IEnumerator SpawnLootWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnLoot();
        spawnEnemy();
    }

    public void SpawnLoot()
    {
        foreach (var cell in _mazeGeneration._mazeCells)
        {
            Vector3 lootPosition = cell.GetRandomPosition(_mazeGeneration._cellSize);
            if (Random.value > spawnChance)
            {
                Instantiate(_lootPrefab[Random.Range(0, _lootPrefab.Count)], lootPosition, Quaternion.identity, LootSpawner);
            }
        }
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
        SpawnLoot();
    }
    public void spawnEnemy()
    {
        foreach (var cell in _mazeGeneration._mazeCells)
        {
            Vector3 lootPosition = cell.GetRandomPosition(_mazeGeneration._cellSize);
            if (Random.value > spawnChance)
            {
                Instantiate(enemySpawnerPrefab[Random.Range(0, enemySpawnerPrefab.Count)], lootPosition, Quaternion.identity, LootSpawner);
            }
        }
    }

}
