using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum MissionType { Slaying, Destination, Escape, Completion }

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;
    public TextMeshProUGUI missionText;
    public Transform playerTransform;

    [SerializeField]
    private List<MissionData> missionsToDefine = new List<MissionData>();


    public class Mission
    {
        public string name;
        public int targetCount;
        public int currentCount;
        public MissionType type;
        public bool isActive;
        public bool isCompleted;
        public Vector3 destinationPosition;

        public float completionTime;
        public float fadeAlpha = 1f;
        public const float fadeDuration = 1.5f;
        public const float displayDuration = 2f;



        public Mission(string name, int targetCount, MissionType type, Vector3 destinationPosition = default)
        {
            this.name = name;
            this.targetCount = targetCount;
            this.currentCount = 0;
            this.type = type;
            this.destinationPosition = destinationPosition;
            this.isActive = false;
            this.isCompleted = false;
        }

        public void IncrementProgress()
        {
            if (isCompleted || !isActive) return;

            currentCount = Mathf.Min(currentCount + 1, targetCount);
            if (currentCount >= targetCount)
                Complete();
        }

        public void Complete()
        {
            if (isCompleted || !isActive) return;

            currentCount = targetCount;
            isCompleted = true;
            completionTime = Time.time;

            DataManager.instance?.activeMissionList.RemoveAll(m => m.name == name);
        }


        public bool ShouldDisplay => isActive && (!isCompleted || (Time.time - completionTime < displayDuration + fadeDuration));

        public string GetDisplayText(Transform player, out float alpha)
        {
            alpha = 1f;

            if (!ShouldDisplay)
            {
                alpha = 0f;
                return "";
            }

            if (isCompleted)
            {
                float t = Time.time - completionTime - displayDuration;
                if (t > 0)
                {
                    alpha = Mathf.Clamp01(1f - (t / fadeDuration));
                }
            }

            string status = $"{name} ({currentCount}/{targetCount})";

            if (type == MissionType.Destination && !isCompleted && player != null)
            {
                float distance = Vector3.Distance(player.position, destinationPosition);
                status += $" ({distance:F0}m)";
            }

            if (isCompleted)
            {
                status += " - Completed";
            }

            return status;
        }
    }

    private Dictionary<string, Mission> allMissions = new Dictionary<string, Mission>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DefineAllMissions();
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        UpdateMissionText();
    }

    void Update()
    {
        foreach (var mission in allMissions.Values)
        {
            if (mission.isActive && !mission.isCompleted && mission.type == MissionType.Destination && playerTransform != null)
            {
                float distance = Vector3.Distance(playerTransform.position, mission.destinationPosition);
                if (distance < 10f)
                {
                    mission.Complete();
                }
            }
        }

        UpdateMissionText();
    }

    public void DefineAllMissions()
    {
        allMissions.Clear();

        foreach (MissionData md in missionsToDefine)
        {
            allMissions[md.missionName] = new Mission(md.missionName, md.targetCount, md.missionType, md.destinationPosition);
        }

        Debug.Log("Defined");
    }

    public void ActivateMission(string missionName, Vector3? destination = null)
    {
        if (allMissions.TryGetValue(missionName, out Mission mission))
        {
            mission.isActive = true;

            if (mission.type == MissionType.Destination && destination.HasValue)
            {
                mission.destinationPosition = destination.Value;
            }

            // Add to DataManager list
            if (!DataManager.instance.activeMissionList.Exists(m => m.name == mission.name))
            {
                DataManager.instance.activeMissionList.Add(
                    new ActiveMissionData(
                        mission.name,
                        mission.type,
                        mission.currentCount,
                        mission.targetCount,
                        mission.destinationPosition
                    )
                );
            }

            UpdateMissionText();
        }
    }


    public void UpdateEnemyKill(string missionName)
    {
        if (allMissions.TryGetValue(missionName, out Mission mission) && mission.isActive && !mission.isCompleted)
        {
            mission.IncrementProgress(); // This updates mission.currentCount++

            var activeData = DataManager.instance.activeMissionList
                .Find(m => m.name == mission.name);

            if (activeData != null)
            {
                activeData.currentCount = mission.currentCount;
            }

            UpdateMissionText();
        }
    }


    public void UpdateDestination(string missionName)
    {
        if (allMissions.TryGetValue(missionName, out Mission mission) && mission.type == MissionType.Destination)
        {
            mission.Complete();
            UpdateMissionText();
        }
    }

    public void CompleteEscape(string missionName)
    {
        if (allMissions.TryGetValue(missionName, out Mission mission) && mission.type == MissionType.Escape)
        {
            mission.Complete();
            UpdateMissionText();
        }
    }

    private void UpdateMissionText()
    {
        if (missionText == null) return;

        string text = "";

        foreach (var mission in allMissions.Values)
        {
            float alpha;
            string display = mission.GetDisplayText(playerTransform, out alpha);

            if (alpha > 0f)
            {
                string hexAlpha = ((int)(alpha * 255)).ToString("X2");
                string colorOpen = $"<color=#FFFFFF{hexAlpha}>";
                string colorClose = "</color>";
                text += $"{colorOpen}{display}{colorClose}\n";
            }
        }

        missionText.text = text;
    }

    public void ReloadActiveMissionsFromDataManager()
    {
        DefineAllMissions(); // Make sure allMissions is populated

        foreach (var missionData in DataManager.instance.activeMissionList)
        {
            if (allMissions.TryGetValue(missionData.name, out Mission mission))
            {
                mission.isActive = true;
                mission.isCompleted = false;

                mission.type = missionData.type;
                mission.currentCount = missionData.currentCount;
                mission.targetCount = missionData.targetCount;

                if (missionData.type == MissionType.Destination)
                    mission.destinationPosition = missionData.destinationPosition;

                UpdateMissionText();
            }
        }
    }


    public void ForceCompleteMission(string missionName)
    {
        if (allMissions.TryGetValue(missionName, out Mission mission) && mission.isActive && !mission.isCompleted)
        {
            mission.Complete();

            var activeData = DataManager.instance.activeMissionList.Find(m => m.name == mission.name);
            if (activeData != null)
            {
                DataManager.instance.activeMissionList.Remove(activeData);
            }

            UpdateMissionText();
        }
    }

    public bool IsMissionCompleted(string missionName)
    {
        if (allMissions.TryGetValue(missionName, out Mission mission))
        {
            return mission.isCompleted;
        }
        return false;
    }

}
