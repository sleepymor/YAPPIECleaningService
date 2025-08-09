using UnityEngine;

public class MissionChainWithPairs : MonoBehaviour
{
    public MissionPair[] missionPairs;

    void Update()
    {
        foreach (var pair in missionPairs)
        {
            if (AreAllMissionsCompleted(pair.missionsToComplete))
            {
                ActivateMissions(pair.missionsToActivate);
                Debug.Log("Aktif");
            }
        }
    }

    bool AreAllMissionsCompleted(string[] missionsToCheck)
    {
        foreach (string missionName in missionsToCheck)
        {
            if (!MissionManager.instance.IsMissionCompleted(missionName))
                return false;
        }
        return true;
    }

    void ActivateMissions(string[] missionsToActivate)
    {
        foreach (string missionName in missionsToActivate)
        {
            MissionManager.instance.ActivateMission(missionName);
        }
    }
}
