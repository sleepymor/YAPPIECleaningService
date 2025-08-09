using UnityEngine;

[System.Serializable]
public class ActiveMissionData
{
    public string name;
    public MissionType type;
    public int currentCount;
    public int targetCount;
    public Vector3 destinationPosition;
    public bool isCompleted;

    public ActiveMissionData(string name, MissionType type, int currentCount, int targetCount, Vector3 destinationPosition, bool isCompleted = false)
    {
        this.name = name;
        this.type = type;
        this.currentCount = currentCount;
        this.targetCount = targetCount;
        this.destinationPosition = destinationPosition;
        this.isCompleted = isCompleted;
    }
}


[System.Serializable]
public class MissionPair
{
    public string[] missionsToComplete;  // misi yang harus selesai dulu
    public string[] missionsToActivate;  // misi yang akan diaktifkan setelah semua misi di atas selesai
}

[System.Serializable]
public class MissionData
{
    public string missionName;
    public int targetCount;
    public MissionType missionType;
    public Vector3 destinationPosition;
}
