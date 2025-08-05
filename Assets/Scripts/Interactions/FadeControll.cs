using UnityEngine;

public class FadeControll : MonoBehaviour
{
    [SerializeField] private Material tilemapMaterial;
    [Range(0f, 1f)] public float fade = 1f;
    private float targetFade;
    private float fadeSpeed = 2f;

    void Start()
    {
        targetFade = fade;
        tilemapMaterial.SetFloat("_Fade", fade);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            targetFade = (targetFade < 0.5f) ? 1f : 0f;
        }

        // Smooth fade
        fade = Mathf.MoveTowards(fade, targetFade, Time.deltaTime * fadeSpeed);
        tilemapMaterial.SetFloat("_Fade", fade);
    }
}
