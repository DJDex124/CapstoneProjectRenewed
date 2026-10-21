using System.Collections.Generic;
using UnityEngine;

public class SpawnLoot2 : MonoBehaviour
{
    public static SpawnLoot2 current { get; private set; }

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

    public int LootCount;      // how many actually spawned
    public int MaxLootCount;   // how many we try to spawn
    public List<OldItemData> lootData = new List<OldItemData>();

    /// <summary>Spawns up to MaxLootCount items on the spawn points of the given loot cells.</summary>
    public void SpawnLootIn(List<MazeCell> lootCells, Transform parent = null)
    {
        LootCount = 0;

        var points = new List<Transform>();
        foreach (MazeCell cell in lootCells)
        {
            if (cell == null) continue;

            if (cell.spawnPoints != null && cell.spawnPoints.Count > 0)
            {
                foreach (GameObject sp in cell.spawnPoints)
                    if (sp != null) points.Add(sp.transform);
            }
            else
            {
                points.Add(cell.transform); // fallback: cell centre
            }
        }

        if (MaxLootCount > points.Count)
            Debug.LogWarning($"SpawnLoot: wanted {MaxLootCount} items but only {points.Count} spawn points exist.");

        int toSpawn = Mathf.Min(MaxLootCount, points.Count);
        for (int i = 0; i < toSpawn; i++)
        {
            int index = Random.Range(0, points.Count);
            Transform point = points[index];
            points.RemoveAt(index); // one item per spawn point

            OldItemData item = PickItem();
            if (item == null || item.pickupPrefab == null) continue;

            Instantiate(item.pickupPrefab, point.position, Quaternion.identity, parent);
            LootCount++;
        }
    }

    // dropChance is treated as a relative weight, so a roll can never come up empty.
    private OldItemData PickItem()
    {
        float total = 0f;
        foreach (OldItemData item in lootData) total += Mathf.Max(0f, item.dropChance);
        if (total <= 0f) return null;

        float roll = Random.value * total;
        foreach (OldItemData item in lootData)
        {
            roll -= Mathf.Max(0f, item.dropChance);
            if (roll <= 0f) return item;
        }
        return lootData[lootData.Count - 1];
    }
}
