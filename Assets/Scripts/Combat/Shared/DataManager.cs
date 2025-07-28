using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    // Player data
    public int PlayerHealth = -99999999;

    // Dictionary to store enemy health data
    private Dictionary<int, int> enemyHealthData = new Dictionary<int, int>();

    // To generate unique IDs
    private int currentEnemyID = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep the DataManager across scenes
        }
        else
        {
            Destroy(gameObject);  // Ensure there's only one DataManager
        }
    }

    // Generate a new unique ID for each enemy and store their health
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
}
