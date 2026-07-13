using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerCreative : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("GameManager", LoadSceneMode.Additive);  
        SceneManager.LoadScene("UIManager", LoadSceneMode.Additive);
        SceneManager.LoadScene("InventorySystem", LoadSceneMode.Additive);
        

        
    }
    public void generateMaze()
    {
        SceneManager.LoadScene("MazeGeneration", LoadSceneMode.Additive);
    }
    public void resetLevel()
    {
        
        SceneManager.UnloadSceneAsync("MazeGeneration");
        generateMaze();
    }
}
