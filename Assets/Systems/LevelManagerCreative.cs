using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerCreative : MonoBehaviour
{
    public static LevelManagerCreative current { get; private set; }

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
    void Start()
    {
        SceneManager.LoadScene("GameManager", LoadSceneMode.Additive);  
        //SceneManager.LoadScene("UIManager", LoadSceneMode.Additive);
        //SceneManager.LoadScene("InventorySystem", LoadSceneMode.Additive);
        SceneManager.LoadScene("MazeGeneration", LoadSceneMode.Additive);


    }
   
    public void resetLevel()
    {
        
        SceneManager.UnloadSceneAsync("MazeGeneration");
        SceneManager.LoadScene("MazeGeneration", LoadSceneMode.Additive);

    }
}
