using UnityEngine;
using System.Collections.Generic;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _frontWall;

    [SerializeField]
    private GameObject _backWall;
    
    [SerializeField]
    private GameObject _unvisitedBlock;

    [SerializeField]
    private GameObject _rockPrefab;

    [Header("Cell Settings")]
    public List<GameObject> spawnPoints;
    public CellType cellType;
    public bool spawned = false;    


    public bool IsVisited { get; private set; }
    public int GridX { get; private set; }
    public int GridZ { get; private set; }
    public void SetGridPosition(int x, int z)
    {
        GridX = x;
        GridZ = z;

        //these two logic checks prevent internal cells from owning a wall preventing overlapping
        if (x > 0)
        {
            _leftWall.SetActive(false);
        }

        if (z > 0)
        {
            _backWall.SetActive(false);
        }
    }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);

    }

    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }

    public void dropRocks(int amount)
    {
        int rockCount = Random.Range(1, amount); 
        for (int i = 0; i < rockCount; i++)
        {
            int randomSizeIndex = Random.Range(1, 80); 
            Vector3 randomPosition = new Vector3(
                Random.Range(-0.5f, 0.5f), 
                16f, // Y position (height)
                Random.Range(-0.5f, 0.5f) 
            );
            GameObject rock = Instantiate(_rockPrefab, transform.position + randomPosition, Quaternion.identity);
            rock.transform.localScale = new Vector3(randomSizeIndex + 50, randomSizeIndex + 50, randomSizeIndex + 50);
        }
    }
}

public enum CellType
{
    basicCell,
    LootCell,
    EnemyCell,
    TrapCell
}