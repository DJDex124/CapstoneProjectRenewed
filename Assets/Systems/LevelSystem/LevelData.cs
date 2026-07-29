using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public int mazeWidthandDepth;
    public int lootCellCount;
    public int enemyCellCount;
    public int trapCellCount;

    public int lootSpawnCount;
    public int enemySpawnCount;
}
