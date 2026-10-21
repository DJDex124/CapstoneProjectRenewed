using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MazeSettings
{
    public int size;          
    public int lootCells;
    public int enemyCells;
    public int trapCells;
    public int parkourCells;
}

/// <summary>
/// One self-contained maze. Entrance is on the west (-X) edge, exit on the east (+X) edge,
/// so mazes chain along +X. Replaces MazeGeneration.
/// </summary>
public class MazeInstance : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private float cellSize = 4f;
    [SerializeField] private List<Vector2Int> blockedCells = new List<Vector2Int>();

    [Header("Cell prefabs")]
    [SerializeField] private MazeCells basicCell;
    [SerializeField] private MazeCells lootCell;
    [SerializeField] private MazeCells enemyCell;
    [SerializeField] private MazeCells trapCell;
    [SerializeField] private MazeCells parkourCell;

    [Header("Generation")]
    [Tooltip("Yield to the next frame every N carve steps (0 = never yield).")]
    [SerializeField] private int stepsPerFrame = 50;

    public readonly List<MazeCells> AllCells = new List<MazeCells>();
    public readonly Dictionary<CellTypes, List<MazeCells>> CellsByType = new Dictionary<CellTypes, List<MazeCells>>();

    private MazeCells[,] grid;
    private HashSet<Vector2Int> blocked;
    private int width, depth, entranceZ, exitZ;

    public Vector3 EntranceWorldPosition => transform.TransformPoint(LocalEdgePoint(0, entranceZ, -1));
    public Vector3 ExitWorldPosition => transform.TransformPoint(LocalEdgePoint(width - 1, exitZ, +1));

    private Vector3 LocalCellCentre(int x, int z) => new Vector3(x * cellSize, 0f, z * cellSize);

    private Vector3 LocalEdgePoint(int x, int z, int xDir) =>
        LocalCellCentre(x, z) + new Vector3(xDir * cellSize * 0.5f, 0f, 0f);

    /// <summary>Builds the maze so its entrance edge sits exactly at entranceWorldPos.</summary>
    public IEnumerator Generate(Vector3 entranceWorldPos, MazeSettings settings, bool openExit)
    {
        width = depth = Mathf.Max(2, settings.size);

        blocked = new HashSet<Vector2Int>(blockedCells);
        blocked.RemoveWhere(p => p.x < 0 || p.x >= width || p.y < 0 || p.y >= depth);

        entranceZ = PickOpenRow(0);
        exitZ = PickOpenRow(width - 1);

        transform.position = entranceWorldPos - LocalEdgePoint(0, entranceZ, -1);

        SpawnCells(settings);
        yield return Carve(grid[0, entranceZ]);

        grid[0, entranceZ].ClearLeftWall();
        if (openExit) grid[width - 1, exitZ].ClearRightWall();

        int visited = 0;
        foreach (MazeCells c in AllCells) if (c.IsVisited) visited++;
        if (visited < AllCells.Count)
            Debug.LogWarning($"{name}: {AllCells.Count - visited} cells unreachable (blockedCells splits the grid?).");
    }

    private int PickOpenRow(int x)
    {
        var rows = new List<int>();
        for (int z = 0; z < depth; z++)
            if (!blocked.Contains(new Vector2Int(x, z))) rows.Add(z);

        if (rows.Count == 0)
        {
            Debug.LogError($"{name}: column {x} is fully blocked, no valid entrance/exit.");
            return 0;
        }
        return rows[Random.Range(0, rows.Count)];
    }

    private void SpawnCells(MazeSettings s)
    {
        grid = new MazeCells[width, depth];
        AllCells.Clear();
        CellsByType.Clear();

        int playable = width * depth - blocked.Count;
        int slots = Mathf.Max(0, playable - 2); // entrance + exit cells stay plain

        var types = new[] { CellTypes.LootCell, CellTypes.EnemyCell, CellTypes.TrapCell, CellTypes.ParkourCell };
        var counts = new[] { s.lootCells, s.enemyCells, s.trapCells, s.parkourCells };

        // If the requested mix doesn't fit, scale every type down proportionally
        // instead of letting the first types starve the later ones.
        int requested = 0;
        foreach (int c in counts) requested += Mathf.Max(0, c);
        float scale = (requested > slots && requested > 0) ? (float)slots / requested : 1f;

        var pool = new List<CellTypes>(slots);
        for (int i = 0; i < types.Length; i++)
        {
            int n = Mathf.FloorToInt(Mathf.Max(0, counts[i]) * scale);
            for (int j = 0; j < n; j++) pool.Add(types[i]);
        }
        while (pool.Count < slots) pool.Add(CellTypes.basicCell);
        Shuffle(pool);

        int next = 0;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (blocked.Contains(new Vector2Int(x, z))) continue;

                bool isDoor = (x == 0 && z == entranceZ) || (x == width - 1 && z == exitZ);
                CellTypes type = (isDoor || next >= pool.Count) ? CellTypes.basicCell : pool[next++];

                MazeCells cell = Instantiate(PrefabFor(type), transform);
                cell.transform.localPosition = LocalCellCentre(x, z);
                cell.transform.localRotation = Quaternion.identity;
                cell.SetGridPosition(x, z);

                grid[x, z] = cell;
                AllCells.Add(cell);

                if (!CellsByType.TryGetValue(type, out var list))
                    CellsByType[type] = list = new List<MazeCells>();
                list.Add(cell);
            }
        }

        // Second pass, once every neighbour is known: drop duplicated walls only where a neighbour exists.
        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
                if (grid[x, z] != null)
                    grid[x, z].RemoveSharedWalls(x > 0 && grid[x - 1, z] != null,
                                                 z > 0 && grid[x, z - 1] != null);
    }

    private MazeCells PrefabFor(CellTypes type)
    {
        MazeCells p = type switch
        {
            CellTypes.LootCell => lootCell,
            CellTypes.EnemyCell => enemyCell,
            CellTypes.TrapCell => trapCell,
            CellTypes.ParkourCell => parkourCell,
            _ => basicCell
        };
        if (p == null)
        {
            Debug.LogWarning($"{name}: no prefab assigned for {type}, using basic cell.");
            p = basicCell;
        }
        return p;
    }

    // Iterative recursive-backtracker: same maze shape as before, no nested coroutines.
    private IEnumerator Carve(MazeCells start)
    {
        var stack = new Stack<MazeCells>();
        var neighbours = new List<MazeCells>(4);
        start.Visit();
        stack.Push(start);

        int steps = 0;
        while (stack.Count > 0)
        {
            MazeCells current = stack.Peek();
            GatherUnvisited(current, neighbours);

            if (neighbours.Count > 0)
            {
                MazeCells next = neighbours[Random.Range(0, neighbours.Count)];
                next.Visit();
                ClearWalls(current, next);
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }

            if (stepsPerFrame > 0 && ++steps % stepsPerFrame == 0)
                yield return null;
        }
    }

    private void GatherUnvisited(MazeCells cell, List<MazeCells> results)
    {
        results.Clear();
        TryAdd(cell.GridX + 1, cell.GridZ, results);
        TryAdd(cell.GridX - 1, cell.GridZ, results);
        TryAdd(cell.GridX, cell.GridZ + 1, results);
        TryAdd(cell.GridX, cell.GridZ - 1, results);
    }

    private void TryAdd(int x, int z, List<MazeCells> results)
    {
        if (x < 0 || x >= width || z < 0 || z >= depth) return;
        MazeCells c = grid[x, z];
        if (c != null && !c.IsVisited) results.Add(c);
    }

    private void ClearWalls(MazeCells a, MazeCells b)
    {
        if (b.GridX > a.GridX) { a.ClearRightWall(); b.ClearLeftWall(); }
        else if (b.GridX < a.GridX) { a.ClearLeftWall(); b.ClearRightWall(); }
        else if (b.GridZ > a.GridZ) { a.ClearFrontWall(); b.ClearBackWall(); }
        else if (b.GridZ < a.GridZ) { a.ClearBackWall(); b.ClearFrontWall(); }
    }

    /// <summary>chance is a percentage (0-100). Rocks are parented to this maze.</summary>
    public void TriggerRockFall(int chance, int stage)
    {
        foreach (MazeCells cell in AllCells)
        {
            if (cell != null && Random.value * 100f < chance)
                cell.dropRocks(stage, transform);
        }
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
