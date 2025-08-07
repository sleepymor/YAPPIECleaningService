using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class GameSaveManager
{
    private static string savePath => Path.Combine(Application.persistentDataPath, "savedata.json");

    public static void SaveGame(DataManager data)
    {
        SaveDataContainer saveData = new SaveDataContainer
        {
            playerHealth = data.PlayerHealth,
            playerPos = data.playerPos
        };

        foreach (var kvp in data.GetAllEnemyHealth())
        {
            saveData.enemies.Add(new EnemyData(kvp.Key, kvp.Value));
        }

        // Save active missions
        saveData.activeMissions = new List<ActiveMissionData>(data.activeMissionList);

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to " + savePath);
    }


    public static void LoadGame(DataManager data)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveDataContainer saveData = JsonUtility.FromJson<SaveDataContainer>(json);

        data.PlayerHealth = saveData.playerHealth;
        data.savedPlayerPos = saveData.playerPos;
        data.ClearAllEnemies();

        foreach (var enemy in saveData.enemies)
        {
            data.ForceSetEnemyHealth(enemy.id, enemy.health);
        }

        // Load active missions
        data.activeMissionList = new List<ActiveMissionData>(saveData.activeMissions);

        Debug.Log("Game loaded from " + savePath);
    }

}


[System.Serializable]
public class SaveDataContainer
{
    public int playerHealth;
    public List<EnemyData> enemies = new List<EnemyData>();
    public Vector2 playerPos;
    public List<ActiveMissionData> activeMissions = new List<ActiveMissionData>();

}

[System.Serializable]
public class EnemyData
{
    public int id;
    public int health;

    public EnemyData(int id, int health)
    {
        this.id = id;
        this.health = health;
    }
}