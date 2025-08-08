using UnityEngine;

public class PathBlocker : MonoBehaviour
{
    public string[] missionsToUnlock;  // misi yang harus selesai dulu

    private Collider2D blockerCollider;
    private SpriteRenderer spriteRenderer;
    private float fadeSpeed = 1f;  // kecepatan fade out (1 detik)
    private bool startFading = false;

    void Awake()
    {
        blockerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("PathBlocker: No SpriteRenderer found.");
        }
    }

    void Update()
    {
        if (AreAllMissionsCompleted())
        {
            if (!startFading)
            {
                startFading = true;
                // Bisa juga disable collider supaya tidak menghalangi segera
                if (blockerCollider != null) blockerCollider.enabled = false;
            }

            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a -= fadeSpeed * Time.deltaTime;
                c.a = Mathf.Clamp01(c.a);
                spriteRenderer.color = c;

                if (c.a <= 0f)
                {
                    gameObject.SetActive(false); // disable object setelah fade selesai
                }
            }
            else
            {
                // Jika tidak ada sprite renderer, langsung disable saja
                gameObject.SetActive(false);
            }
        }
        else
        {
            // Jika misi belum selesai, pastikan objek dan collider aktif dan alpha normal
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (blockerCollider != null && !blockerCollider.enabled)
                blockerCollider.enabled = true;

            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                if (c.a < 1f)
                {
                    c.a += fadeSpeed * Time.deltaTime;
                    c.a = Mathf.Clamp01(c.a);
                    spriteRenderer.color = c;
                }
            }
            startFading = false;
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
