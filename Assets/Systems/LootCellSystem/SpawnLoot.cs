using System.Collections.Generic;
using UnityEngine;

public class SpawnLoot : MonoBehaviour
{
    public static SpawnLoot current { get; private set; }
    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
        }
        else
        {
            current = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public int LootCount;
    public int MaxLootCount;
    public int maxLootCellCount = 10;
    public int lootCellCount = 0;

    public List<GameObject> lootCells;
    public List<GameObject> SpawnPoints;
    public List<OldItemData> lootData= new List<OldItemData>();
    
    public void findLootCells()
    {
        LootCount = 0;
        lootCellCount = 0;
        lootCells.Clear();
        SpawnPoints.Clear();
        GameObject[] lootCellsArray = GameObject.FindGameObjectsWithTag("LootCell");
        foreach (GameObject cell in lootCellsArray)
        {
            
            if ( lootCellCount <= maxLootCellCount)
            {
                lootCells.Add(cell);
                lootCellCount++;
            }
        }

        spawnRandomLoot();
    }

    void spawnRandomLoot()
    {
        // use this if I want spawns to be randomly everywhere and if I want more loot to spawn than loot cells
        foreach (GameObject cell in lootCells)
        {
            foreach (Transform child in cell.transform)
            {
                if (child.CompareTag("SpawnPoint"))
                    SpawnPoints.Add(child.gameObject);
            }
        }
        for (int i = 0; i < MaxLootCount; i++) //repeat until i = MaxLootCount
        { 
            Debug.Log("Spawning Loot: " + i);
            int randomIndex = Random.Range(0, SpawnPoints.Count);
            GameObject spawnPosition = SpawnPoints[randomIndex];
            SpawnPoints.RemoveAt(randomIndex);

            int randomLootIndex = Random.Range(0, lootData.Count);
            GameObject lootPrefab = changeDatatoObject();
            Instantiate(lootPrefab, spawnPosition.transform.position, Quaternion.identity);
            LootCount++;

        }
    }
    
    GameObject changeDatatoObject()
    {
        OldItemData loot = GetItems();
        GameObject Loot = loot.pickupPrefab;
        return Loot;
    }


    OldItemData GetItems()
    {
        int randomNumber = Random.Range(1, 101);
        List <OldItemData> possibleItems = new List <OldItemData>();
        foreach (OldItemData item in lootData)
        { 
           if (randomNumber <= item.dropChance)
           {
                possibleItems.Add(item);
           }
        }
        if (possibleItems.Count > 0)
        {
            OldItemData selectedLoot = possibleItems[Random.Range(0, possibleItems.Count)];
            return selectedLoot;
        }
        return null;

    }
}
    

