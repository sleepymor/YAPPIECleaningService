using UnityEngine;

public class ChargingStation : MonoBehaviour, InteractableInterface
{

    public void Interract()
    {
        PlayerCombat.instance.ChargeFull();
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
