using UnityEngine;

[System.Serializable]
public class ActiveMissionData
{
    public string name;
    public MissionType type;
    public int currentCount;
    public int targetCount;
    public Vector3 destinationPosition;

    public ActiveMissionData(string name, MissionType type, int currentCount, int targetCount, Vector3 destinationPosition)
    {
        this.name = name;
        this.type = type;
        this.currentCount = currentCount;
        this.targetCount = targetCount;
        this.destinationPosition = destinationPosition;
    }
}
