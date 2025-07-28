using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;

    private Dictionary<EnemyEnvironment, int> totalEnemies = new();
    private Dictionary<EnemyEnvironment, int> killedEnemies = new();
    private Dictionary<EnemyEnvironment, float> environmentProgress = new();

    void Awake()
    {
        Instance = this;

        foreach (EnemyEnvironment env in System.Enum.GetValues(typeof(EnemyEnvironment)))
        {
            totalEnemies[env] = 0;
            killedEnemies[env] = 0;
            environmentProgress[env] = 0f;
        }
    }

    public void RegisterEnemy(EnemyEnvironment env)
    {
        if (!totalEnemies.ContainsKey(env))
            totalEnemies[env] = 0;

        totalEnemies[env]++;
    }

    public void EnemyKilled(EnemyEnvironment env)
    {
        if (!killedEnemies.ContainsKey(env))
            killedEnemies[env] = 0;

        killedEnemies[env]++;

        float progressPerKill = 100f / totalEnemies[env];
        environmentProgress[env] += progressPerKill;

        environmentProgress[env] = Mathf.Clamp(environmentProgress[env], 0, 100);

        Debug.Log($"[{env}] Progress: {environmentProgress[env]}%");
    }

    public float GetEnvironmentProgress(EnemyEnvironment env)
    {
        return environmentProgress.ContainsKey(env) ? environmentProgress[env] : 0f;
    }
}
