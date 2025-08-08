using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;
    [SerializeField] private DataManager dataManager;

    public void Continue()
    {
        GameSaveManager.LoadGame(dataManager);
        LoadScene(DataManager.instance.playerScene);
    }

    public void BackToMainMenu()
    {
        LoadScene("MainMenu");
    }


    private void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAndUnloadMainMenu(sceneName));
    }

    private IEnumerator LoadAndUnloadMainMenu(string sceneName)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            yield return null;
        }

        if (progressBar != null)
            progressBar.value = 1f;

        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone)
            yield return null;
        SceneManager.UnloadSceneAsync("GameOver");

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}
