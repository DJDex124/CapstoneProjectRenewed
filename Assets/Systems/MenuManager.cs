using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void startButton()
    {
        SceneManager.LoadScene("Gameplay Display");
    }
    public void quitButton()
    {
        Application.Quit();
    }
    public void mainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void resumeButton()
    {
        GameManager.current.ResumeGame();
    }
    public void pauseButton()
    {
        GameManager.current.PauseGame();
    }
    
}
