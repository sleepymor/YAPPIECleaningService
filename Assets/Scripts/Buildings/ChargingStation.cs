using UnityEngine;

public class ChargingStation : MonoBehaviour, InteractableInterface
{

    public void Interract()
    {
        PlayerCombat.instance.ChargeFull();
    }

    public bool CanInterract()
    {
        return true;
    }

    public string InteractionName()
    {
        return "Charge?";
    }

    private void UseStation()
    {

    }
}
