using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{ 

    public void StartNewGame()
    {
        SceneManager.LoadScene("Level Design");
    }

    public void Continue()
    {
        SceneManager.LoadScene("Level Design");
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
