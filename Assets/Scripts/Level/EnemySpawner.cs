using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int maxSpawn = 10;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private bool spawnInfinitely = true;  // Toggle: true = keep spawning, false = spawn once up to max

    private float timer;
    private List<GameObject> activeEnemies = new List<GameObject>();
    [SerializeField] private Collider2D spawnArea;
    [SerializeField] private Transform enemiesParent;

    private bool hasReachedLimit = false;

    void Start()
    {
        if (enemiesParent == null)
        {
            GameObject enemiesGO = GameObject.Find("Enemies");
            if (enemiesGO != null)
            {
                enemiesParent = enemiesGO.transform;
            }
            else
            {
                Debug.LogWarning("Enemies GameObject not found in scene. Enemies will be spawned without parent.");
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Clean up null enemies to keep list accurate
        CleanupDestroyedEnemies();

        if (spawnInfinitely)
        {
            // Spawn continuously while under maxSpawn
            if (timer >= spawnInterval && activeEnemies.Count < maxSpawn)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }
        else
        {
            // Spawn only once until maxSpawn reached
            if (!hasReachedLimit && timer >= spawnInterval && activeEnemies.Count < maxSpawn)
            {
                SpawnEnemy();
                timer = 0f;
            }

            if (activeEnemies.Count >= maxSpawn)
            {
                hasReachedLimit = true;  // Stop spawning further
            }
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = GetRandomPointInBounds();

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        if (enemiesParent != null)
        {
            newEnemy.transform.parent = enemiesParent;
        }

        activeEnemies.Add(newEnemy);
    }

    void CleanupDestroyedEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    Vector2 GetRandomPointInBounds()
    {
        Bounds bounds = spawnArea.bounds;
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }

    void OnDrawGizmosSelected()
    {
        if (spawnArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
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
}
