using UnityEngine;
using System.Collections;

public enum AttackMode { Melee, Ranged, AOE }

public class Enemy_Attack : MonoBehaviour
{
    private int meleeDamage;
    private int rangedDamage;
    private int AOEDamage;
    private AttackMode attackMode;
    private AttackMode defaultFallbackMode;
    private GameObject projectilePrefab;
    private int projectileCount;
    private float spreadAngle;
    private float rangedProjectileSpeed;
    private float maxProjTravelDistance;

    private GameObject aoePrefab;
    private float aoeCooldown = 5f;
    private bool firstAOETriggered = false;


    [Header("AOE Spam Settings")]
    private float aoeFireRate;
    private float aoeDuration;
    private float aoeProjectileSpeed;
    private float aoeDamageInterval;
    private float maxAoeLifetime;


    private AttackStatus meleeAttackStatus;
    private float meleeAttackStatusChance;
    private float meleeAttackStatusDuration;

    private AttackStatus rangedAttackStatus;
    private float rangedAttackStatusChance;
    private float rangedAttackStatusDuration;

    private AttackStatus aoeAttackStatus;
    private float aoeAttackStatusChance;
    private float aoeAttackStatusDuration;


    public AttackMode CurrentAttackMode => attackMode;

    private float lastAOETimestamp = -Mathf.Infinity;

    private Animator animator;
    private Enemy_Detection enemyDetection;
    private Enemy_Config config;
    private float knockbackRange;

    private void Awake()
    {
        config = GetComponent<Enemy_Config>();
        animator = GetComponent<Animator>();
        enemyDetection = GetComponent<Enemy_Detection>();

        meleeDamage = config.MeleeDamage;
        rangedDamage = config.RangedDamage;
        AOEDamage = config.AOEDamage;
        attackMode = config.Skill;
        defaultFallbackMode = config.BasicAttack;
        projectilePrefab = config.ProjectilePrefab;
        projectileCount = config.ProjectileCount;
        rangedProjectileSpeed = config.RangedProjectileSpeed;
        maxProjTravelDistance = config.MaxProjTravelDistance;
        spreadAngle = config.SpreadAngle;

        meleeAttackStatus = config.MeleeAttackStatus;
        meleeAttackStatusChance = config.MeleeAttackStatusChance;
        meleeAttackStatusDuration = config.MeleeAttackStatusDuration;

        rangedAttackStatus = config.RangedAttackStatus;
        rangedAttackStatusChance = config.RangedAttackStatusChance;
        rangedAttackStatusDuration = config.RangedAttackStatusDuration;

        aoeAttackStatus = config.AOEAttackStatus;
        aoeAttackStatusChance = config.AOEAttackStatusChance;
        aoeAttackStatusDuration = config.AOEAttackStatusDuration;

        aoePrefab = config.AOEPrefab;
        aoeDuration = config.AOEDuration;
        aoeFireRate = config.AOEFireRate;
        firstAOETriggered = config.ImmediatelyTriggered; 
        maxAoeLifetime = config.MaxAOELifetime;
        aoeProjectileSpeed = config.AOEProjectileSpeed;
        aoeDamageInterval = config.AOEDamageInterval;

        knockbackRange = config.KnockbackRange;
    }

    public void OnDamageSent()
    {
        //Debug.Log($"[{name}] OnDamageSent triggered. Mode: {attackMode}");

        if (enemyDetection == null || enemyDetection.currentTarget == null)
        {
            //Debug.LogWarning("Enemy detection or target missing.");
            return;
        }

        if (PlayerCombat.instance == null)
        {
            //Debug.LogWarning("PlayerCombat instance missing.");
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
                    //Debug.LogWarning("AOE attack selected but prefab is not assigned.");
                    UseFallback(end);
                    return;
                }

                if (!firstAOETriggered || Time.time >= lastAOETimestamp + aoeCooldown)
                {
                    TriggerAOE();
                    lastAOETimestamp = Time.time;
                    firstAOETriggered = true; 
                }
                else
                {
                    //Debug.Log("AOE on cooldown, using fallback.");
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
            //Debug.Log("Target out of range.");
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
        //Debug.Log("Performing melee attack.");

        PlayerCombat.instance.TakeDamage(meleeDamage, transform, knockbackRange, meleeAttackStatus, meleeAttackStatusChance, meleeAttackStatusDuration);
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
                projectile.damage = rangedDamage;
                projectile.SetDamageType(DamageType.OnHit);
                projectile.damageInterval = 0;
                projectile.attackStatus = rangedAttackStatus;
                projectile.statusChance = rangedAttackStatusChance;
                projectile.statusDuration = rangedAttackStatusDuration;
                projectile.knockbackRange = knockbackRange;
                projectile.maxTravelDistance = maxProjTravelDistance;
                projectile.aoeLifetime = 0;
                projectile.speed = rangedProjectileSpeed;
                projectile.Initialize(shootDir);
            }
        }
    }

    private void TriggerAOE()
    {
        if (aoePrefab == null)
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

                GameObject proj = Instantiate(aoePrefab, spawnPos, Quaternion.identity);
                Enemy_Projectiles projectile = proj.GetComponent<Enemy_Projectiles>();
                if (projectile != null)
                {
                    projectile.damage = AOEDamage;
                    projectile.damageInterval = aoeDamageInterval;
                    projectile.SetDamageType(DamageType.PerSecond);
                    projectile.maxTravelDistance = 0;
                    projectile.attackStatus = aoeAttackStatus;
                    projectile.statusChance = aoeAttackStatusChance;
                    projectile.statusDuration = aoeAttackStatusDuration;
                    projectile.knockbackRange = knockbackRange;
                    projectile.aoeLifetime = maxAoeLifetime;
                    projectile.speed = aoeProjectileSpeed;
                    projectile.Initialize(shootDir);
                }
            }

            yield return new WaitForSeconds(aoeFireRate);
            elapsed += aoeFireRate;
        }
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
