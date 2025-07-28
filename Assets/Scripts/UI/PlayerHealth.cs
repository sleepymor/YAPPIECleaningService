using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public TextMeshProUGUI healthMeterText;

    void Update()
    {
        healthMeterText.text = "Yappie Health : " + PlayerCombat.instance.currentHealth.ToString();
    }
}