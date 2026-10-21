using UnityEngine;
using System.Collections.Generic;

public class MazeCells : MonoBehaviour
{
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _frontWall;
    [SerializeField] private GameObject _backWall;
    [SerializeField] private GameObject _unvisitedBlock;
    [SerializeField] private GameObject _rockPrefab;

    [Header("Cell Settings")]
    [Tooltip("Where loot / items can spawn in this cell. Drag the spawn point children in here.")]
    public List<GameObject> spawnPoints;
    public CellTypes cellType;
    public bool spawned = false;

    [Header("Rock Fall")]
    [SerializeField] private float rockDropHeight = 16f;
    [SerializeField] private float rockScaleMin = 51f;
    [SerializeField] private float rockScaleMax = 130f;

    public bool IsVisited { get; private set; }
    public int GridX { get; private set; }
    public int GridZ { get; private set; }

    /// <summary>Only stores coordinates. No side effects.</summary>
    public void SetGridPosition(int x, int z)
    {
        GridX = x;
        GridZ = z;
    }

    /// <summary>
    /// Each shared wall is owned by one cell (the left/back cell keeps its right/front wall).
    /// Only drop our left/back wall if a cell actually exists on that side,
    /// otherwise blocked or missing neighbours leave a hole.
    /// </summary>
    public void RemoveSharedWalls(bool hasLeftNeighbour, bool hasBackNeighbour)
    {
        if (hasLeftNeighbour && _leftWall != null) _leftWall.SetActive(false);
        if (hasBackNeighbour && _backWall != null) _backWall.SetActive(false);
    }

    public void Visit()
    {
        IsVisited = true;
        if (_unvisitedBlock != null) _unvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall() { if (_leftWall != null) _leftWall.SetActive(false); }
    public void ClearRightWall() { if (_rightWall != null) _rightWall.SetActive(false); }
    public void ClearFrontWall() { if (_frontWall != null) _frontWall.SetActive(false); }
    public void ClearBackWall() { if (_backWall != null) _backWall.SetActive(false); }

    /// <summary>Drops 1..maxRocks rocks. Pass a parent so they are cleaned up with the level.</summary>
    public void dropRocks(int maxRocks, Transform parent = null)
    {
        if (_rockPrefab == null) return;

        int rockCount = Random.Range(1, maxRocks + 1); // int max is exclusive
        for (int i = 0; i < rockCount; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                rockDropHeight,
                Random.Range(-0.5f, 0.5f));

            GameObject rock = Instantiate(_rockPrefab, transform.position + offset, Quaternion.identity, parent);
            rock.transform.localScale = Vector3.one * Random.Range(rockScaleMin, rockScaleMax);
        }
    }
}

// New entries go at the END so existing serialized values don't shift.
public enum CellTypes
{
    basicCell,
    LootCell,
    EnemyCell,
    TrapCell,
    ParkourCell
}