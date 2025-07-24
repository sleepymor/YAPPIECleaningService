using UnityEngine;

public class ChargingStation : MonoBehaviour, InteractableInterface
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
