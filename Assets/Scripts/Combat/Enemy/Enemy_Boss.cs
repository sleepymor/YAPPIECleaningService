using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{
    private int meleeDamage;
    private int rangedDamage;

    private GameObject projectilePrefab;
    private int projectileCount;
    private float spreadAngle;
    private float rangedProjectileSpeed;
    private float maxProjTravelDistance;


    private AttackStatus meleeAttackStatus;
    private float meleeAttackStatusChance;
    private float meleeAttackStatusDuration;

    private AttackStatus rangedAttackStatus;
    private float rangedAttackStatusChance;
    private float rangedAttackStatusDuration;

    private Animator animator;
    private Enemy_Detection enemyDetection;
    private Enemy_Config config;
    private float knockbackRange;

    public AreaDamage smallArea;
    public AreaDamage bigArea;

    void Awake()
    {
        config = GetComponent<Enemy_Config>();
        animator = GetComponent<Animator>();

        config = GetComponent<Enemy_Config>();
        animator = GetComponent<Animator>();
        enemyDetection = GetComponent<Enemy_Detection>();

        meleeDamage = config.MeleeDamage;
        rangedDamage = config.RangedDamage;
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


        knockbackRange = config.KnockbackRange;

        smallArea.gameObject.SetActive(false);
        bigArea.gameObject.SetActive(false);

    }
    public void randomAttack()
    {
        stopAttack();
        float rand = Random.Range(0f, 100f);

        //if (rand < 10f)
        //{
        //    powerStompAttack();
        //}
        //else if (rand < 40f)
        //{
        //    crushAttack();
        //}
        //else if (rand < 60f)
        //{
        //    stompAttack();
        //}
        //else
        //{
        //    meleeAttack();
        //}

        powerStompAttack();
    }

    public void stopAttack()
    {
        if (animator) animator.SetBool("stompAttack", false);
        if (animator) animator.SetBool("powerstompAttack", false);
        if (animator) animator.SetBool("meleeAttack", false);
        if (animator) animator.SetBool("crushAttack", false);

        Vector2 start = transform.position;
        Vector2 end = enemyDetection.currentTarget.position;
        float distanceToTarget = Vector2.Distance(start, end);

        if (distanceToTarget <= 10f)
        {
            PlayerCombat.instance.TakeDamage(meleeDamage, transform, knockbackRange, meleeAttackStatus, meleeAttackStatusChance, meleeAttackStatusDuration);
        }
        if (distanceToTarget <= 20f)
        {
            PlayerCombat.instance.TakeDamage(meleeDamage, transform, knockbackRange, meleeAttackStatus, meleeAttackStatusChance, meleeAttackStatusDuration);
        }
        smallArea.gameObject.SetActive(false);
        bigArea.gameObject.SetActive(false);
    }

    private void stompAttack()
    {
        if (animator) animator.SetBool("stompAttack", true);
    }

    private void powerStompAttack()
    {
        if (animator) animator.SetBool("powerstompAttack", true);
        bigArea.gameObject.SetActive(true);
    }

    private void meleeAttack()
    {
        if (animator) animator.SetBool("meleeAttack", true);
    }

    private void crushAttack()
    {
        if (animator) animator.SetBool("crushAttack", true);
    }

    public void OnMeleeAttackSends()
    {
        if (projectilePrefab == null || enemyDetection == null || enemyDetection.currentTarget == null)
            return;

        Vector2 targetPosition = enemyDetection.currentTarget.position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        Vector2 spawnPos = (Vector2)transform.position + direction * 0.5f;

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
            projectile.Initialize(direction);
        }
    }


    public void OnStompAttackSends()
    {
        smallArea.gameObject.SetActive(true);
    }

    public void OnPowerStompAttackSends()
    {
    }



    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        Vector2 targetPosition = enemyDetection.currentTarget.position;
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



}
