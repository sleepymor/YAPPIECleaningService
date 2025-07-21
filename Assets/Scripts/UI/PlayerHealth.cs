using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public TextMeshProUGUI healthMeterText;

    // Update is called once per frame
    void Update()
    {
        healthMeterText.text = "Yappie Health : " + PlayerCombat.instance.currentHealth.ToString();
    }
}
