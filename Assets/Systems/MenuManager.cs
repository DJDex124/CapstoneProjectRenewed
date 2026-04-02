using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void startButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay Display");
    }
    public void quitButton()
    {
        Application.Quit();
    }
    public void mainMenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
