using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void startButton()
    {
        SceneManager.LoadScene("Gameplay Display");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
    public void Score()
    {
        //SceneManager.LoadScene("Score");
        // open canvas that keeps score or open scene with score
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
