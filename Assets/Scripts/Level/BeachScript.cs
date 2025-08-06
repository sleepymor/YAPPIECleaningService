using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class BeachScript : MonoBehaviour
{
    private float BeachEnvHealth = 0;

    [Header("References")]
    [SerializeField] public Transform beachArea;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private AudioSource beachAudio;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isVisible = true;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        beachArea.gameObject.SetActive(true);

 
        if (beachAudio != null)
        {
            beachAudio.loop = true;
            beachAudio.playOnAwake = false;
            beachAudio.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Hitbox" && beachAudio != null)
        {
            if (!beachAudio.isPlaying)
            {
                beachAudio.Play();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.name == "Hitbox")
        {
            BeachEnvHealth = EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.Beach);
            EnvironmentBar.instance.SetArrowPosition(BeachEnvHealth);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Hitbox" && beachAudio != null)
        {
            if (beachAudio.isPlaying)
            {
                StartCoroutine(FadeOutAudio(beachAudio, fadeDuration));
            }
        }
    }

    void Update()
    {
        if (EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.Beach) > 99)
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

            fadeCoroutine = StartCoroutine(FadeTilemap(false));

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

    private IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume for future use
    }
}
