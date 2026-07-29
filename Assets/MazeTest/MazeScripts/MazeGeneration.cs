using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class MazeGeneration : MonoBehaviour
{
    public static MazeGeneration current {  get; private set; }

    private void Awake()
    {
        current = this;
    }


    [SerializeField]
    private MazeCell _mazeCellPrefab;
    [Header("cell Settings")]
    public MazeCell LootCell;
    public MazeCell EnemyCell;
    public MazeCell TrapCell;
    public MazeCell BasicCell;
    public MazeCell spawnCell;
    public List <MazeCell> cellsToSpawn;

    public Transform playerSpawn;

    public int maxlootCellAmount = 10;
    public int currentlootCellAmount;

    public int maxEnemyCellAmount = 15;
    public int currentEnemyCellAmount;

    public int maxTrapCellAmount = 10;
    public int currentTrapCellAmount;

    public float _cellSize = 4f;

    public int _mazeWidth;

    public int _mazeDepth;

    public Vector2Int _spawnPosition;

    public Transform mazePos;

    private MazeCell[,] _mazeGrid;
    public List<MazeCell> _mazeCells = new List<MazeCell>();


    [SerializeField] private int safeZone = 1;
    private HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>();

    public bool createExit = false;
    public void addCells()
    {
        
        while (currentEnemyCellAmount < maxEnemyCellAmount || currentlootCellAmount < maxlootCellAmount || currentTrapCellAmount < maxTrapCellAmount)
        {
            if (currentlootCellAmount < maxlootCellAmount)
            {
                cellsToSpawn.Add(LootCell);
                currentlootCellAmount++;
                
            }
            if (currentEnemyCellAmount < maxEnemyCellAmount)
            {
                cellsToSpawn.Add(EnemyCell);
                currentEnemyCellAmount++;
                
            }
            if (currentTrapCellAmount < maxTrapCellAmount)
            {
                cellsToSpawn.Add(TrapCell);
                currentTrapCellAmount++;
                
            }
        }
        
    }

  

    public IEnumerator StartMazeGeneration()
    {
        addCells();
        

        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        int centerX = _mazeWidth / 2;
        int centerZ = _mazeDepth / 2;

        int halfSize = safeZone / 2;

        for (int x = centerX - halfSize; x < centerX + halfSize; x++)
        {
            for (int z = centerZ - halfSize; z < centerZ + halfSize; z++)
            {
                blockedCells.Add(new Vector2Int(x, z));
            }
        }
        int totalSpawnable = (_mazeWidth * _mazeDepth) - blockedCells.Count;
        while (totalSpawnable > cellsToSpawn.Count)
        {
            cellsToSpawn.Add(BasicCell);
        }

        cellsToSpawn.Shuffle();

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);

                // Skip the safe zone
                if (blockedCells.Contains(pos))
                    continue;

                MazeCell prefabToUse;

                if (cellsToSpawn.Count > 0)
                {
                    prefabToUse = cellsToSpawn[0];
                    cellsToSpawn.RemoveAt(0);
                }
                else
                {
                    prefabToUse = BasicCell;
                }

                _mazeGrid[x, z] = Instantiate(
                    prefabToUse,
                    new Vector3(x * _cellSize, 0, z * _cellSize),
                    Quaternion.identity,
                    mazePos);

                _mazeCells.Add(_mazeGrid[x, z]);
            }
        }

        Vector3 spawnPosition = new Vector3(centerX * _cellSize, 0, centerZ * _cellSize);

        _mazeGrid[centerX, centerZ] = Instantiate(spawnCell, spawnPosition, Quaternion.identity, mazePos);

        _mazeCells.Add(_mazeGrid[centerX, centerZ]);


        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = spawnPosition + Vector3.up;
        }

        MazeCell startCell = _mazeCells[Random.Range(0, _mazeCells.Count)];

        yield return GenerateMaze(null, startCell);

        CreateEntranceAndExit();
    }

    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        //yield return new WaitForSeconds(0.04f);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = Mathf.RoundToInt(currentCell.transform.position.x / _cellSize);
        int z = Mathf.RoundToInt(currentCell.transform.position.z / _cellSize);

        Vector2Int checkPos;

        // right
        checkPos = new Vector2Int(x + 1, z);
        if (x + 1 < _mazeWidth && !blockedCells.Contains(checkPos))
        {
            var cell = _mazeGrid[x + 1, z];
            if (cell != null && !cell.IsVisited)
                yield return cell;
        }
        // left
        checkPos = new Vector2Int(x - 1, z);
        if (x - 1 >= 0 && !blockedCells.Contains(checkPos))
        {
            var cell = _mazeGrid[x - 1, z];
            if (cell != null && !cell.IsVisited)
                yield return cell;
        }

        // front
        checkPos = new Vector2Int(x, z + 1);
        if (z + 1 < _mazeDepth && !blockedCells.Contains(checkPos))
        {
            var cell = _mazeGrid[x, z + 1];
            if (cell != null && !cell.IsVisited)
                yield return cell;
        }

        // bcak
        checkPos = new Vector2Int(x, z - 1);
        if (z - 1 >= 0 && !blockedCells.Contains(checkPos))
        {
            var cell = _mazeGrid[x, z - 1];
            if (cell != null && !cell.IsVisited)
                yield return cell;
        }
    }
    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
            return;

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if(previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;  
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();   
            return;
        }
    }

    private void CreateEntranceAndExit()
    {
        int centerX = _mazeWidth / 2;
        int centerZ = _mazeDepth / 2;
        int halfSize = safeZone / 2;

        //safe zone enter maze 
        Vector2Int entrancePos = new Vector2Int(centerX, centerZ - halfSize - 1);
        MazeCell entranceCell = _mazeGrid[entrancePos.x, entrancePos.y];

        if (entranceCell != null)
        {
            entranceCell.ClearFrontWall(); 
        }

        List<Vector2Int> edgeCells = new List<Vector2Int>();

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                // skip null / safe zone
                if (_mazeGrid[x, z] == null)
                    continue;

                bool isEdge = x == 0 || x == _mazeWidth - 1 || z == 0 || z == _mazeDepth - 1;

                if (isEdge)
                {
                    edgeCells.Add(new Vector2Int(x, z));
                }
            }
        }

        if ( createExit)
        {
            Vector2Int exitPos = edgeCells[Random.Range(0, edgeCells.Count)];
            MazeCell exitCell = _mazeGrid[exitPos.x, exitPos.y];

            if (exitPos.x == 0)
            {
                exitCell.ClearLeftWall();
            }
            else if (exitPos.x == _mazeWidth - 1)
            {
                exitCell.ClearRightWall();
            }
            else if (exitPos.y == 0)
            {
                exitCell.ClearBackWall();
            }
            else if (exitPos.y == _mazeDepth - 1)
            {
                exitCell.ClearFrontWall();
            }
        }
    }
}


public static class listExtentions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
