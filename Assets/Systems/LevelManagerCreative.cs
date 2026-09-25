using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManagerCreative : MonoBehaviour
{
    public static LevelManagerCreative current { get; private set; }
    public bool sceneReady { get; private set; } = false;

    void Awake()
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

    // No auto-load in Start() anymore — GameManager drives this explicitly.

    public void resetLevel()
    {
        StartCoroutine(resetLevelRoutine());
    }

    private IEnumerator resetLevelRoutine()
    {
        sceneReady = false;

        Scene existing = SceneManager.GetSceneByName("MazeGeneration");
        if (existing.IsValid() && existing.isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("MazeGeneration");
            yield return unloadOp; // wait until fully unloaded
        }

        AsyncOperation loadOp = SceneManager.LoadSceneAsync("MazeGeneration", LoadSceneMode.Additive);
        yield return loadOp; // wait until fully loaded

        sceneReady = true;
    }
}
