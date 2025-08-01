using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class PlayerHealth : MonoBehaviour
{
    public TextMeshProUGUI healthMeterText;
    public Slider healthSlider;

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
        }

        MissionManager.instance.ActivateMission("Slay 5 Monsters");

        List<string> dialog = new List<string>{
                "Welcome to the cave.",
                "It’s dangerous to go alone.",
                "Take this sword and good luck!"
        };

        DialogManager.instance.StartDialog(dialog, 4f);
    }

    void Update()
    {
        float currentHealth = PlayerCombat.instance.currentHealth;
        float startingHealth = PlayerCombat.instance.startingHealth;

        float healthPercent = Mathf.Clamp01(currentHealth / startingHealth);

        if (healthSlider != null)
            healthSlider.value = healthPercent;

        if (healthMeterText != null)
            healthMeterText.text = Mathf.RoundToInt(healthPercent * 100f).ToString() + "%";
    }
}
