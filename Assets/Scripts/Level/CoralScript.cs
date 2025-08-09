using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class CoralScript : MonoBehaviour
{
    private float CoralEnvHealth = 0;

    [Header("References")]
    [SerializeField] public Transform coralArea;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private AudioSource coralAudio;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isVisible = true;
    private Coroutine fadeCoroutine;


    public string completionMission;


    private void Awake()
    {
        coralArea.gameObject.SetActive(true);


        if (coralAudio != null)
        {
            coralAudio.loop = true;
            coralAudio.playOnAwake = false;
            coralAudio.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && coralAudio != null)
        {
            if (!coralAudio.isPlaying)
            {
                coralAudio.Play();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CoralEnvHealth = EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.DeadCoralReefs);
            EnvironmentBar.instance.SetArrowPosition(CoralEnvHealth);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && coralAudio != null)
        {
            if (coralAudio.isPlaying)
            {
                StartCoroutine(FadeOutAudio(coralAudio, fadeDuration));
            }
        }
    }

    void Update()
    {
        if (EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.DeadCoralReefs) > 99)
        {
            ToggleTrash();
            MissionManager.instance.ForceCompleteMission(completionMission);

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
