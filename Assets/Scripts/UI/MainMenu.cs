using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public GameObject loadingScreen;
    [SerializeField] public Slider progressBar;

    public void StartNewGame()
    {
        LoadScene("MainGame");
    }

    public void Continue()
    {
        LoadScene("SampleScene");
    }

    public void QuitApp()
    {
        Application.Quit();
    }


    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            yield return null;
        }
    }
}
