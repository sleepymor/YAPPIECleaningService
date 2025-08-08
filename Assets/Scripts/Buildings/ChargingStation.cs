using UnityEngine;
using UnityEngine.SceneManagement;


public class ChargingStation : MonoBehaviour, InteractableInterface
{
    public string missionName;

    public void Interract()
    {
        PlayerCombat.instance.ChargeFull();
        DataManager.instance.playerScene = SceneManager.GetActiveScene().name;
        DataManager.instance.Save();

        if (missionName != null)
        {
            MissionManager.instance.ForceCompleteMission(missionName);
        }
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
