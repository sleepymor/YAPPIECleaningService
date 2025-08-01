using UnityEngine;

public class DestinationMission : MonoBehaviour
{
    [SerializeField] private string missionName;

    void Start()
    {
        if (!string.IsNullOrEmpty(missionName))
        {
            MissionManager.instance.ActivateMission(missionName, transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(missionName) && other.transform.name == "Hitbox" && other.transform.root.GetComponent<PlayerController>() != null)
        {
            MissionManager.instance.UpdateDestination(missionName);
        }
    }
}
