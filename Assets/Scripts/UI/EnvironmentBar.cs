using UnityEngine;
using UnityEngine.UI;

public class EnvironmentBar : MonoBehaviour
{
    public static EnvironmentBar instance;
    [SerializeField] private RectTransform topArrow;
    [SerializeField] private RectTransform bottomArrow;
    [SerializeField] private RectTransform bar; // Make sure this is Bar, not EnvBar
    [SerializeField] public float arrPos;


    public void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Set arrow positions based on 0–100 value
    /// </summary>
    /// <param name="value">Expected from 0 to 100</param>
    public void SetArrowPosition(float value)
    {
        value = Mathf.Clamp01(value / 100f);

        float barWidth = bar.rect.width;
        float pivotOffset = -barWidth * bar.pivot.x;

        float arrowWidth = topArrow.rect.width;

        // Left edge moves from 0 to (barWidth - arrowWidth)
        float x = Mathf.Lerp(barWidth - arrowWidth, 0f, value);
        float finalX = pivotOffset + x;

        topArrow.anchoredPosition = new Vector2(finalX, topArrow.anchoredPosition.y);
        bottomArrow.anchoredPosition = new Vector2(finalX, bottomArrow.anchoredPosition.y);
    }
}

