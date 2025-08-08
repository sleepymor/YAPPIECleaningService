using UnityEngine;

public class MissionTriggers : MonoBehaviour
{
    public Collider2D triggerArea;  // renamed from 'transform' to avoid confusion
    public string[] missionNames;   // multiple missions

    private bool isActivated = false;

    void Awake()
    {
        if (triggerArea != null)
            triggerArea.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            foreach (var missionName in missionNames)
            {
                MissionManager.instance.ActivateMission(missionName);
            }
            isActivated = true;
        }
    }
}
