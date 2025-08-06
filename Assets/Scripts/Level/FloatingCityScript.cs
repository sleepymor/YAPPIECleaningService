using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class FloatingCityScript : MonoBehaviour
{
    public static float FloatingCityEnvHealth = 0;
    [SerializeField] public Transform floatCityArea;
    [SerializeField] private Tilemap[] tilemaps;
    [SerializeField] private float fadeDuration = 0.5f;
    private bool isVisible = true;
    private Coroutine fadeCoroutine;

    public void Awake()
    {
        floatCityArea.gameObject.SetActive(true);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.name == "Hitbox")
        {
            FloatingCityEnvHealth = EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.FloatingCity);
            EnvironmentBar.instance.SetArrowPosition(FloatingCityEnvHealth);
            //Debug.Log(FloatingCityEnvHealth);
        }
    }

    void Update()
    {
        if (EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.FloatingCity) > 99)
        {
            ToggleTrash();
        }
    }


    public void ToggleTrash()
    {
        if (isVisible)
        {
            isVisible = false;
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeTilemaps(isVisible));
        }
    }

    private IEnumerator FadeTilemaps(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        // Store original colors
        Color[] originalColors = new Color[tilemaps.Length];
        float[] startAlphas = new float[tilemaps.Length];

        for (int i = 0; i < tilemaps.Length; i++)
        {
            originalColors[i] = tilemaps[i].color;
            startAlphas[i] = tilemaps[i].color.a;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            for (int i = 0; i < tilemaps.Length; i++)
            {
                float newAlpha = Mathf.Lerp(startAlphas[i], targetAlpha, t);
                Color c = originalColors[i];
                tilemaps[i].color = new Color(c.r, c.g, c.b, newAlpha);
            }

            yield return null;
        }

        // Set final alpha
        for (int i = 0; i < tilemaps.Length; i++)
        {
            Color c = originalColors[i];
            tilemaps[i].color = new Color(c.r, c.g, c.b, targetAlpha);
        }
    }

}
