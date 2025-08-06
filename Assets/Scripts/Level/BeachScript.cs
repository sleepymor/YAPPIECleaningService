using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class BeachScript : MonoBehaviour
{
    private float BeachEnvHealth = 0;
    [SerializeField] public Transform beachArea;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float fadeDuration = 0.5f;
    private bool isVisible = true;
    private Coroutine fadeCoroutine;

    public void Awake()
    {
        beachArea.gameObject.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.name == "Hitbox")
        {
            BeachEnvHealth = EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.Beach);
            EnvironmentBar.instance.SetArrowPosition(BeachEnvHealth);
        }
    }

    void Update()
    {
        if(EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.Beach) > 99)
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

            fadeCoroutine = StartCoroutine(FadeTilemap(isVisible));
        }
    }

    private IEnumerator FadeTilemap(bool fadeIn)
    {
        float startAlpha = tilemap.color.a;
        float targetAlpha = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        Color originalColor = tilemap.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            tilemap.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null;
        }

        tilemap.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
    }
}
