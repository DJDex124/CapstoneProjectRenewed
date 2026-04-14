using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public Transform LootSpawner;
    public List<GameObject>_lootPrefab;
    [SerializeField]
    private MazeGeneration _mazeGeneration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject go in _lootPrefab) 
        {
            ItemPrefab itemPrefab = go.GetComponent<ItemPrefab>();
            float spawnChance = itemPrefab != null ? itemPrefab.spawnChance : 0.3f; // Default to 0.3 if no ItemPrefab component
            if (spawnChance > 0f)
            {
                SpawnLoot();
            }
        }
    }

    public void SpawnLoot()
    {
        foreach (var cell in _mazeGeneration._mazeCells)
        {

            Vector3 lootPosition = cell.GetRandomPosition(_mazeGeneration._cellSize);
            Instantiate(_lootPrefab[Random.Range(0, _lootPrefab.Count)], lootPosition, Quaternion.identity, LootSpawner);
           
        }
    }
}
