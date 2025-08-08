using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AreaChanger : MonoBehaviour
{
    [Tooltip("Change to your target scene")]
    [SerializeField] private string sceneName = "";
    [SerializeField] private GameObject loadingScreen;
    private Slider progressBar;

    void Start()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        collider2D.gameObject.SetActive(true);

        loadingScreen.SetActive(false); // Hide it at start

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DataManager.instance.isChangingArea = true;
            DataManager.instance.playerScene = "";
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
