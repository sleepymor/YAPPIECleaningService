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
        if (other.CompareTag("Player"))
        {
            MissionManager.instance.UpdateDestination(missionName);
        }
    }
}
