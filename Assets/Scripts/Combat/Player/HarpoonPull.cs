using UnityEngine;

public class HarpoonPull : MonoBehaviour
{
    private bool hasHit = false;
    private float lastHitTime = -999f;

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int damageAmount = PlayerConfig.c.HarpoonDamage;
        float reuseCooldown = PlayerConfig.c.HarpoonCooldown;

        if (hasHit || Time.time - lastHitTime < reuseCooldown) return;

        Enemy_Health[] enemies = FindObjectsOfType<Enemy_Health>();

        if (enemies.Length == 0) return;

        Enemy_Health nearestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy_Health enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && other.gameObject == nearestEnemy.gameObject)
        {
            nearestEnemy.PullDamage(damageAmount);
            hasHit = true;
            lastHitTime = Time.time;
        }
    }
}
