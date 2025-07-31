using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public static SkillUI instance;
    [SerializeField] private Slider dashSlider;
    [SerializeField] private Slider harpoonSlider;
    [SerializeField] private Slider radarSlider;

    [Header("Cooldown Durations (in seconds)")]
    private float dashCooldown;
    private float harpoonCooldown;
    private float radarCooldown;

    private float dashTimer = 0f;
    private float harpoonTimer = 0f;
    private float radarTimer = 0f;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        dashCooldown = PlayerConfig.c.DashCooldown;
        harpoonCooldown = PlayerConfig.c.HarpoonCooldown;
        radarCooldown = 0f;

        InitSlider(dashSlider);
        InitSlider(harpoonSlider);
        InitSlider(radarSlider);
    }

    private void Update()
    {
        HandleCooldown(ref dashTimer, dashCooldown, dashSlider);
        HandleCooldown(ref harpoonTimer, harpoonCooldown, harpoonSlider);
        HandleCooldown(ref radarTimer, radarCooldown, radarSlider);
    }

    private void InitSlider(Slider slider)
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = 0f;
            slider.gameObject.SetActive(false);
        }
    }

    private void HandleCooldown(ref float timer, float cooldown, Slider slider)
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (slider != null)
            {
                slider.gameObject.SetActive(true);
                float percent = Mathf.Clamp01(1f - (timer / cooldown)) * 100f;
                slider.value = percent;

                if (timer <= 0f)
                {
                    slider.value = 100f;
                    slider.gameObject.SetActive(false);
                    timer = 0f;
                }
            }
        }
    }

    // Call these methods to trigger cooldowns externally
    public void TriggerDashCooldown()
    {
        dashTimer = dashCooldown;
        if (dashSlider != null)
        {
            dashSlider.value = 0f;
            dashSlider.gameObject.SetActive(true);
        }
    }

    public void TriggerHarpoonCooldown()
    {
        harpoonTimer = harpoonCooldown;
        if (harpoonSlider != null)
        {
            harpoonSlider.value = 0f;
            harpoonSlider.gameObject.SetActive(true);
        }
    }

    public void TriggerRadarCooldown()
    {
        radarTimer = radarCooldown;
        if (radarSlider != null)
        {
            radarSlider.value = 0f;
            radarSlider.gameObject.SetActive(true);
        }
    }
}
