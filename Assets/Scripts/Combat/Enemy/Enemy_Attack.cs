using UnityEngine;
using System.Collections;

public enum AttackMode { Melee, Ranged, AOE }

public class Enemy_Attack : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private AttackMode attackMode = AttackMode.Melee;
    [SerializeField] private AttackMode defaultFallbackMode = AttackMode.Melee;

    [Header("Projectile Settings (Ranged)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int projectileCount = 5;
    [SerializeField] private float spreadAngle = 30f;

    [Header("AOE Settings")]
    [SerializeField] private GameObject aoePrefab;
    [SerializeField] private float aoeCooldown = 5f;
    [SerializeField] private bool firstAOETriggered = false;


    [Header("AOE Spam Settings")]
    [SerializeField] private float aoeFireRate = 0.1f; // Time between each projectile
    [SerializeField] private float aoeDuration = 4f;    // Total time to spam projectiles
    [SerializeField] private float aoeProjectileSpeed = 2f; // Speed for vomit projectiles

    public AttackMode CurrentAttackMode => attackMode;

    private float lastAOETimestamp = -Mathf.Infinity;

    private Animator animator;
    private Enemy_Detection enemyDetection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyDetection = GetComponent<Enemy_Detection>();
    }

    public void OnDamageSent()
    {
        Debug.Log($"[{name}] OnDamageSent triggered. Mode: {attackMode}");

        if (enemyDetection == null || enemyDetection.currentTarget == null)
        {
            Debug.LogWarning("Enemy detection or target missing.");
            return;
        }

        if (PlayerCombat.instance == null)
        {
            Debug.LogWarning("PlayerCombat instance missing.");
            return;
        }

        Vector2 start = transform.position;
        Vector2 end = enemyDetection.currentTarget.position;
        float distanceToTarget = Vector2.Distance(start, end);

        float effectiveRange = attackMode switch
        {
            AttackMode.Melee => enemyDetection.meleeRange,
            AttackMode.Ranged => enemyDetection.rangedRange,
            AttackMode.AOE => enemyDetection.rangedRange,
            _ => enemyDetection.meleeRange
        };

        if (distanceToTarget <= effectiveRange)
        {
            if (attackMode == AttackMode.AOE)
            {
                if (aoePrefab == null)
                {
                    Debug.LogWarning("AOE attack selected but prefab is not assigned.");
                    UseFallback(end);
                    return;
                }

                // Allow first AOE to fire without cooldown
                if (!firstAOETriggered || Time.time >= lastAOETimestamp + aoeCooldown)
                {
                    TriggerAOE();
                    lastAOETimestamp = Time.time;
                    firstAOETriggered = true; // after first shot, enable cooldown
                }
                else
                {
                    Debug.Log("AOE on cooldown, using fallback.");
                    UseFallback(end);
                }
            }

            else
            {
                ExecuteAttackMode(attackMode, end);
            }
        }
        else
        {
            Debug.Log("Target out of range.");
        }
    }

    private void ExecuteAttackMode(AttackMode mode, Vector2 target)
    {
        switch (mode)
        {
            case AttackMode.Melee:
                DoMeleeAttack();
                break;
            case AttackMode.Ranged:
                FireProjectile(target);
                break;
        }
    }

    private void UseFallback(Vector2 target)
    {
        ExecuteAttackMode(defaultFallbackMode, target);
    }

    private void DoMeleeAttack()
    {
        Debug.Log("Performing melee attack.");
        PlayerCombat.instance.TakeDamage(damage, transform);
    }

    private void FireProjectile(Vector2 targetPosition)
    {
        if (projectilePrefab == null) return;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float halfSpread = spreadAngle / 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = Mathf.Lerp(-halfSpread, halfSpread, (float)i / (projectileCount - 1));
            float angle = baseAngle + angleOffset;

            Vector2 shootDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 spawnPos = (Vector2)transform.position + shootDir * 0.5f;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Enemy_Projectiles projectile = proj.GetComponent<Enemy_Projectiles>();
            if (projectile != null)
            {
                projectile.damage = damage;
                projectile.Initialize(shootDir);
            }
        }
    }

    private void TriggerAOE()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("AOE projectilePrefab missing.");
            return;
        }

        StartCoroutine(SpamAOEProjectiles());
    }

    private IEnumerator SpamAOEProjectiles()
    {
        float elapsed = 0f;

        Vector2 baseDirection = (enemyDetection.currentTarget.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
        float halfSpread = spreadAngle / 2f;

        while (elapsed < aoeDuration)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                float angleOffset = Mathf.Lerp(-halfSpread, halfSpread, (float)i / (projectileCount - 1));
                float angle = baseAngle + angleOffset;
                Vector2 shootDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                Vector2 spawnPos = (Vector2)transform.position + shootDir * 0.5f;

                GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                Enemy_Projectiles projectile = proj.GetComponent<Enemy_Projectiles>();
                if (projectile != null)
                {
                    projectile.damage = damage;
                    projectile.Initialize(shootDir);
                    projectile.SetDamageType(DamageType.PerSecond);
                    projectile.speed = aoeProjectileSpeed; // override speed
                }
            }

            yield return new WaitForSeconds(aoeFireRate);
            elapsed += aoeFireRate;
        }

        Debug.Log("AOE spam complete.");
    }


    public bool IsAOEOrRanged()
    {
        return attackMode == AttackMode.Ranged || attackMode == AttackMode.AOE;
    }

    public bool IsAOEReady()
    {
        if (attackMode != AttackMode.AOE) return false;
        return !firstAOETriggered || Time.time >= lastAOETimestamp + aoeCooldown;
    }




    public void Attack()
    {
        if (animator) animator.SetBool("attack", true);
    }

    public void StopAttack()
    {
        if (animator) animator.SetBool("attack", false);
    }
}
