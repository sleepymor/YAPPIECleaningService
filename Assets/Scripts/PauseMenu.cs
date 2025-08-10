using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PaufseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject loadingScreen;
    public Slider progressBar;

    public Controller controls;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        if (isPaused && Input.GetKeyDown(KeyCode.X))
        {
            BackToMainMenu();
        }
    }

    void Awake()
    {
        controls = new Controller();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public void Pause()
    {
        pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        PlayerController.instance.canMove = false;
        controls.Disable();
    }

    public void Resume()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        PlayerController.instance.canMove = true;
        controls.Enable();
    }



    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // reset time before loading
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

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
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
    }
}
