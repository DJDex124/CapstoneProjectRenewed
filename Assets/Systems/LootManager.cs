using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject>_lootPrefab;
    [SerializeField]
    private MazeGeneration _mazeGeneration;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnLoot();
        }
    }

    public void SpawnLoot()
    {
        foreach (var cell in _mazeGeneration._mazeCells)
        {
            Vector3 lootPosition = cell.GetRandomPosition(_mazeGeneration._cellSize);
            Instantiate(_lootPrefab[Random.Range(0, _lootPrefab.Count)], lootPosition, Quaternion.identity);
        }
    }
}
