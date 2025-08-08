using UnityEngine;
using UnityEngine.SceneManagement;


public class ChargingStation : MonoBehaviour, InteractableInterface
{

    public void Interract()
    {
        PlayerCombat.instance.ChargeFull();
        DataManager.instance.playerScene = SceneManager.GetActiveScene().name;
        DataManager.instance.Save();
    }

    public bool CanInterract()
    {
        return true;
    }

    public string InteractionName()
    {
        return "Charge?";
    }
}
