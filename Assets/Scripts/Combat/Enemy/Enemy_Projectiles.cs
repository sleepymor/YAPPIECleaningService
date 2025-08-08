using UnityEngine;

public enum DamageType { OnHit, PerSecond }

public class Enemy_Projectiles : MonoBehaviour
{
    public int damage;
    private DamageType damageType { get; set; }
    public float damageInterval { get; set; }

    [Header("Movement Settings")]
    public float speed { get; set; }
    public float maxTravelDistance { get; set; }
    public float knockbackRange { get; set; }

    public AttackStatus attackStatus { get; set; }
    public float statusChance { get; set; }
    public float statusDuration { get; set; }

    public float aoeLifetime { get; set; }
    private float spawnTime;

    private Vector2 direction;
    private Vector2 startPosition;
    private float lastDamageTime;

    // New properties for chaser
    public bool isChasing = false;
    public Transform chaseTarget;

    public void SetDamageType(DamageType type)
    {
        damageType = type;
    }

    private void Awake()
    {
        lastDamageTime = Time.time - damageInterval;
    }

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        startPosition = transform.position;
        spawnTime = Time.time;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        if (isChasing && chaseTarget != null)
        {
            // Update direction toward target position every frame
            Vector2 targetPos = chaseTarget.position;
            Vector2 currentPos = transform.position;
            direction = (targetPos - currentPos).normalized;

            // Rotate bullet to face direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Move bullet forward
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (damageType == DamageType.PerSecond)
        {
            if (Time.time - spawnTime >= aoeLifetime)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (Vector2.Distance(startPosition, transform.position) >= maxTravelDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageType != DamageType.OnHit) return;

        if (other.CompareTag("Player"))
        {
            PlayerCombat.instance.TakeDamage(damage, transform, knockbackRange, attackStatus, statusChance, statusDuration);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (damageType != DamageType.PerSecond) return;

        if (other.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageInterval)
            {
                lastDamageTime = Time.time;

                if (PlayerCombat.instance != null)
                {
                    PlayerCombat.instance.TakeDamage(damage, transform, knockbackRange, attackStatus, statusChance, statusDuration);
                }
            }
        }
    }
}
