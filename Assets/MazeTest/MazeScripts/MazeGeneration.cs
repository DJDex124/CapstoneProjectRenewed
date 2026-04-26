using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGeneration : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    public float _cellSize = 4f;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    public Vector2Int _spawnPosition;

    public Transform mazePos;

    private MazeCell[,] _mazeGrid;
    public List<MazeCell> _mazeCells = new List<MazeCell>();

    [SerializeField] private int safeZone = 4;
    private HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>();

    IEnumerator Start()
    {
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

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);

                if (blockedCells.Contains(pos))
                    continue;

                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab,
                    new Vector3(x * _cellSize, 0, z * _cellSize),
                    Quaternion.identity, mazePos);

                _mazeCells.Add(_mazeGrid[x, z]);
            }
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
