using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// Replaces MazeGeneration. Lives in the (reloaded) MazeGeneration scene.
/// Builds Maze -> Room -> Maze -> ... along +X from a LevelData.
/// Everything it spawns is parented to this object so it is cleaned up with the level.
/// </summary>
public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder current { get; private set; }

    private void Awake() { current = this; }
    private void OnDestroy() { if (current == this) current = null; }

    [Header("Layout")]
    [Tooltip("Where the first maze's entrance docks (e.g. the door of the player's base).")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private bool openFinalExit = false;

    [Header("Prefabs")]
    [SerializeField] private MazeInstance mazePrefab;
    [SerializeField] private List<RoomPrefab> roomPrefabs = new List<RoomPrefab>();

    [Header("Navigation")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    private readonly List<MazeInstance> mazes = new List<MazeInstance>();
    private readonly List<GameObject> rooms = new List<GameObject>();

    public IReadOnlyList<MazeInstance> Mazes => mazes;
    public bool IsBuilt { get; private set; }

    public IEnumerator BuildLevel(LevelData level)
    {
        IsBuilt = false;
        Clear();

        int count = Mathf.Max(1, level.mazeCount);
        Vector3 nextEntrance = startPoint != null ? startPoint.position : transform.position;

        for (int i = 0; i < count; i++)
        {
            bool last = i == count - 1;

            MazeInstance maze = Instantiate(mazePrefab, transform);
            mazes.Add(maze);
            yield return maze.Generate(nextEntrance, SettingsFor(level, i, count), !last || openFinalExit);

            if (last) break;

            if (roomPrefabs.Count == 0)
            {
                nextEntrance = maze.ExitWorldPosition; // no rooms: mazes dock directly
                continue;
            }

            RoomPrefab room = Instantiate(PickRoom(), transform);
            rooms.Add(room.gameObject);
            room.AlignEntryTo(maze.ExitWorldPosition);
            nextEntrance = room.exitSocket.position;

            yield return null;
        }

        // Bake once, after everything exists.
        if (navMeshSurface != null) navMeshSurface.BuildNavMesh();
        IsBuilt = true;
    }

    // Splits level-wide cell counts across mazes; the remainder goes to the first mazes.
    private static MazeSettings SettingsFor(LevelData l, int index, int count) => new MazeSettings
    {
        size = l.mazeWidthandDepth,
        lootCells = Share(l.lootCellCount, index, count),
        enemyCells = Share(l.enemyCellCount, index, count),
        trapCells = Share(l.trapCellCount, index, count),
        parkourCells = Share(l.parkourCellCount, index, count)
    };

    private static int Share(int total, int index, int count) =>
        total / count + (index < total % count ? 1 : 0);

    public List<MazeCells> GetCells(CellTypes type)
    {
        var result = new List<MazeCells>();
        foreach (MazeInstance m in mazes)
            if (m.CellsByType.TryGetValue(type, out var list)) result.AddRange(list);
        return result;
    }

    public void TriggerRockFall(int chance, int stage)
    {
        foreach (MazeInstance m in mazes) m.TriggerRockFall(chance, stage);
    }

    private RoomPrefab PickRoom()
    {
        int total = 0;
        foreach (RoomPrefab r in roomPrefabs) total += r.weight;

        int roll = Random.Range(0, total);
        foreach (RoomPrefab r in roomPrefabs)
        {
            roll -= r.weight;
            if (roll < 0) return r;
        }
        return roomPrefabs[0];
    }

    public void Clear()
    {
        foreach (MazeInstance m in mazes) if (m != null) Destroy(m.gameObject);
        foreach (GameObject r in rooms) if (r != null) Destroy(r);
        mazes.Clear();
        rooms.Clear();
    }
}