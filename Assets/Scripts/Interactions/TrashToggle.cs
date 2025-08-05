using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class TrashToggle : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isVisible = true;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap is not assigned.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTrash();
        }
    }

    public void ToggleTrash()
    {
        isVisible = !isVisible;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTilemap(isVisible));
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
