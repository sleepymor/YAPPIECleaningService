using UnityEngine;

public class ForceGrayscale : MonoBehaviour
{
    private SpriteRenderer sr;
    private bool forceGrey = false;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    public void EnableGreyscale()
    {
        forceGrey = true;
    }

    public void DisableGreyscale()
    {
        forceGrey = false;
        if (sr != null)
            sr.color = originalColor;
    }

    void LateUpdate()
    {
        if (forceGrey && sr != null)
        {
            float average = (sr.color.r + sr.color.g + sr.color.b) / 3f;
            sr.color = new Color(average, average, average, sr.color.a);
        }
    }
}
