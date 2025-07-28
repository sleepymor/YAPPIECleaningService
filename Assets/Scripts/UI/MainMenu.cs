using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{ 

    public void StartNewGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void Continue()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
