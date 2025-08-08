using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    public static DataManager instance;
    public Vector2 playerPos;
    public Vector2 savedPlayerPos;
    public string playerScene = "MainGame";

    public bool isChangingArea = false;

    public int PlayerHealth = -99999999;

    private Dictionary<int, int> enemyHealthData = new Dictionary<int, int>();

    public List<ActiveMissionData> activeMissionList = new List<ActiveMissionData>();


    private int currentEnemyID = 0;

    void Awake()
    {
        transform.SetParent(null);
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // Ensure there's only one DataManager
        }

    }

    public void Save()
    {
        GameSaveManager.SaveGame(this);
    }

    public void Load()
    {
        //GameSaveManager.LoadGame(this);
        //MissionManager.instance?.ReloadActiveMissionsFromDataManager();
    }

    public int RegisterEnemyHealth(int health)
    {
        int enemyID = currentEnemyID++;  // Assign the next unique ID
        enemyHealthData.Add(enemyID, health);  // Store health
        return enemyID;
    }

    // Get the health of an enemy by their unique ID
    public int GetEnemyHealth(int enemyID)
    {
        if (enemyHealthData.ContainsKey(enemyID))
        {
            return enemyHealthData[enemyID];  // Return health if the ID exists
        }
        else
        {
            Debug.LogWarning("Enemy ID not found: " + enemyID);
            return -1;  // Return -1 if the ID doesn't exist (you could choose a different fallback)
        }
    }

    // Update an enemy's health based on their ID
    public void UpdateEnemyHealth(int enemyID, int health)
    {
        if (enemyHealthData.ContainsKey(enemyID))
        {
            enemyHealthData[enemyID] = health;  // Update the health
        }
        else
        {
            Debug.LogWarning("Enemy ID not found for update: " + enemyID);
        }
    }

    // Remove an enemy's data when they are destroyed (optional)
    public void RemoveEnemyData(int enemyID)
    {
        if (enemyHealthData.ContainsKey(enemyID))
        {
            enemyHealthData.Remove(enemyID);
        }
        else
        {
            Debug.LogWarning("Enemy ID not found to remove: " + enemyID);
        }
    }

    public Dictionary<int, int> GetAllEnemyHealth()
    {
        return new Dictionary<int, int>(enemyHealthData);
    }

    public void ClearAllEnemies()
    {
        enemyHealthData.Clear();
        currentEnemyID = 0;
    }

    // Used only during load to avoid triggering new IDs
    public void ForceSetEnemyHealth(int id, int health)
    {
        enemyHealthData[id] = health;
        if (id >= currentEnemyID)
            currentEnemyID = id + 1;
    }


}

