using UnityEngine;

public class HarpoonPull : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float reuseCooldown = 0.2f;

    private bool hasHit = false;
    private float lastHitTime = -999f;

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit || Time.time - lastHitTime < reuseCooldown) return;

        Enemy_Health[] enemies = FindObjectsOfType<Enemy_Health>();

        if (enemies.Length == 0) return;

        // Find nearest enemy to this harpoon
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
