using System.Collections.Generic;
using UnityEngine;

public class EnemySystem2 : MonoBehaviour
{
    public static EnemySystem2 current { get; private set; }

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<GameObject> Enemy;      // enemy prefabs
    public int EnemyCount;              // how many are alive from this spawn
    public int MaxEnemyCount;           // cap per level (0 = one per enemy cell)

    private readonly List<GameObject> spawned = new List<GameObject>();

    /// <summary>One enemy per enemy cell, capped at MaxEnemyCount if that is above 0.</summary>
    public void SpawnEnemiesIn(List<MazeCell> enemyCells, Transform parent = null)
    {
        removeEnemies();

        if (Enemy == null || Enemy.Count == 0)
        {
            Debug.LogWarning("EnemySystem: no enemy prefabs assigned.");
            return;
        }

        foreach (MazeCell cell in enemyCells)
        {
            if (cell == null) continue;
            if (MaxEnemyCount > 0 && EnemyCount >= MaxEnemyCount) break;

            GameObject prefab = Enemy[Random.Range(0, Enemy.Count)];
            spawned.Add(Instantiate(prefab, cell.transform.position, Quaternion.identity, parent));
            EnemyCount++;
        }
    }

    public void removeEnemies()
    {
        foreach (GameObject enemy in spawned)
            if (enemy != null) Destroy(enemy);

        spawned.Clear();
        EnemyCount = 0;
    }
}
