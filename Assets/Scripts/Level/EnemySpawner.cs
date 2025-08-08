using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int maxSpawn = 10;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private GameObject enemyPrefab;

    private float timer;
    private List<GameObject> activeEnemies = new List<GameObject>();
    [SerializeField] private Collider2D spawnArea;

    void Start()
    {
        spawnArea = GetComponent<Collider2D>();
        spawnArea.isTrigger = true; 
    }

    void Update()
    {
        CleanupDestroyedEnemies();

        timer += Time.deltaTime;

        if (timer >= spawnInterval && activeEnemies.Count < maxSpawn)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = GetRandomPointInBounds();

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemy_Pathfinding pathfinding = enemyObj.GetComponent<Enemy_Pathfinding>();
        pathfinding.stopHide();
        pathfinding.MoveTo(spawnPos.normalized);

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
}
