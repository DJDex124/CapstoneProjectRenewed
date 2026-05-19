using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeMenuGeneration : MonoBehaviour
{
    [Header("Maze Settings")]
    [SerializeField] private MazeCell mazeCellPrefab;

    [SerializeField] private int mazeWidth = 80;
    [SerializeField] private int mazeDepth = 80;

    [SerializeField] private float cellSize = 4f;

    [SerializeField] private Transform mazeParent;

    private MazeCell[,] mazeGrid;
    private List<MazeCell> mazeCells = new List<MazeCell>();

    private void Start()
    {
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        // Create all cells
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                MazeCell newCell = Instantiate(
                    mazeCellPrefab,
                    new Vector3(x * cellSize, 0f, z * cellSize),
                    Quaternion.identity,
                    mazeParent
                );

                mazeGrid[x, z] = newCell;
                mazeCells.Add(newCell);
            }
        }

        // Pick random start cell
        MazeCell startCell = mazeCells[Random.Range(0, mazeCells.Count)];

        // Generate instantly
        GeneratePath(null, startCell);
    }

    private void GeneratePath(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();

        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GeneratePath(currentCell, nextCell);
            }

        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells
            .OrderBy(_ => Random.Range(0, 100))
            .FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = Mathf.RoundToInt(currentCell.transform.position.x / cellSize);
        int z = Mathf.RoundToInt(currentCell.transform.position.z / cellSize);

        // RIGHT
        if (x + 1 < mazeWidth)
        {
            MazeCell cell = mazeGrid[x + 1, z];

            if (cell != null && !cell.IsVisited)
            {
                yield return cell;
            }
        }

        // LEFT
        if (x - 1 >= 0)
        {
            MazeCell cell = mazeGrid[x - 1, z];

            if (cell != null && !cell.IsVisited)
            {
                yield return cell;
            }
        }

        // FRONT
        if (z + 1 < mazeDepth)
        {
            MazeCell cell = mazeGrid[x, z + 1];

            if (cell != null && !cell.IsVisited)
            {
                yield return cell;
            }
        }

        // BACK
        if (z - 1 >= 0)
        {
            MazeCell cell = mazeGrid[x, z - 1];

            if (cell != null && !cell.IsVisited)
            {
                yield return cell;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
            return;

        // RIGHT
        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        // LEFT
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        // FRONT
        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        // BACK
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }


}
