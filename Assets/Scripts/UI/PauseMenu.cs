using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance; // <--- Singleton instance

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseBtn;
    public bool isPaused = false;

    private RectTransform pausePanelRect;
    private RectTransform pauseBtnRect;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (isPaused && Input.GetKeyDown(KeyCode.X))
        {
            BackToMenu();
        }
    }

    private void Awake()
    {
        // Setup singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);

        pausePanelRect = pausePanel.GetComponent<RectTransform>();
        pauseBtnRect = pauseBtn.GetComponent<RectTransform>();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        PlayerController.instance.controller.Disable();
        PlayerCombat.instance.combat.Disable();
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        PlayerController.instance.controller.Enable();
        PlayerCombat.instance.combat.Enable();
        pausePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public bool IsMouseOverPauseUI()
    {
        if (pauseBtn != null && pauseBtn.activeInHierarchy &&
            RectTransformUtility.RectangleContainsScreenPoint(pauseBtnRect, Input.mousePosition, null))
        {
            return true;
        }

        if (pausePanel != null && pausePanel.activeInHierarchy &&
            RectTransformUtility.RectangleContainsScreenPoint(pausePanelRect, Input.mousePosition, null))
        {
            return true;
        }

        return false;
    }
}
