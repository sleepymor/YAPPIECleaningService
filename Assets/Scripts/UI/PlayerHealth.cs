using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public TextMeshProUGUI healthMeterText;

    void Update()
    {
        healthMeterText.text = "X " + PlayerCombat.instance.currentHealth.ToString();
    }
}