using System.Collections;
using UnityEngine;

/// <summary>
/// Test-only driver for an isolated scene. Builds a level on Start and gives you a rebuild button.
/// Needs nothing from GameManager, SpawnLoot, EnemySystem or EndDevice.
/// </summary>
public class MazeChainTester : MonoBehaviour
{
    [SerializeField] private LevelBuilder builder;
    [SerializeField] private LevelData level;
    [SerializeField] private bool buildOnStart = true;

    private bool building;

    private IEnumerator Start()
    {
        if (buildOnStart) yield return Rebuild();
    }

    public IEnumerator Rebuild()
    {
        if (building) yield break;
        building = true;

        float startTime = Time.realtimeSinceStartup;
        yield return builder.BuildLevel(level);
        Debug.Log($"Built {builder.Mazes.Count} maze(s) in {Time.realtimeSinceStartup - startTime:F2}s");

        building = false;
    }

    private void OnGUI()
    {
        if (!building && GUI.Button(new Rect(10, 10, 160, 40), "Rebuild level"))
            StartCoroutine(Rebuild());
    }
}