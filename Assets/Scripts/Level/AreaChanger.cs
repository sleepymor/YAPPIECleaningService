using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaChanger : MonoBehaviour
{
    [Tooltip("Change to your target scene")]
    [SerializeField] private string sceneName = "";

    void Awake()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        collider2D.gameObject.SetActive(true);
    }

    public void LoadScene()
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (!scene.isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            scene = SceneManager.GetSceneByName(sceneName);
        }

        if (SceneManager.GetActiveScene().name != scene.name)
        {
            SceneManager.SetActiveScene(scene);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Hitbox" && PlayerCombat.instance != null)
        {
            LoadScene();
        }
    }
}
