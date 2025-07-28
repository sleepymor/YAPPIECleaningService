using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AreaChanger : MonoBehaviour
{
    [Tooltip("Change to your target scene")]
    [SerializeField] private string sceneName = "";
    private GameObject loadingScreen;
    private Slider progressBar;

    void Awake()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        collider2D.gameObject.SetActive(true);

        if (loadingScreen == null)
        {
            GameObject found = GameObject.Find("LoadingScreen");
            if (found != null)
            {
                loadingScreen = found;
                progressBar = loadingScreen.GetComponentInChildren<Slider>();
                loadingScreen.SetActive(false); // Hide it at start
            }
            else
            {
                Debug.LogWarning("LoadingScreen GameObject not found in scene!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Hitbox" && PlayerCombat.instance != null)
        {
            LoadScene(sceneName);
        }
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
