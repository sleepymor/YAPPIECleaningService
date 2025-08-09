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

    public string[] missionsToUnlock;  // misi yang harus selesai dulu

    void Start()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        collider2D.gameObject.SetActive(true);
        loadingScreen.SetActive(false);
    }

    public string missionName;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && AreAllMissionsCompleted())
        {
            DataManager.instance.isChangingArea = true;
            DataManager.instance.playerScene = "";
            MissionManager.instance.ForceCompleteMission(missionName);
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

    bool AreAllMissionsCompleted()
    {
        foreach (string missionName in missionsToUnlock)
        {
            if (!MissionManager.instance.IsMissionCompleted(missionName))
                return false;
        }
        return true;
    }
}
