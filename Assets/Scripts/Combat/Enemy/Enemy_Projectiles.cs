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
        if (damageType == DamageType.PerSecond)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Time.time - spawnTime >= aoeLifetime)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector2.Distance(startPosition, transform.position) >= maxTravelDistance)
            {
                Destroy(gameObject);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageType != DamageType.OnHit) return;

        if (other.transform.name == "Hitbox" && other.transform.root.GetComponent<PlayerController>() != null)
        {
            PlayerCombat.instance.TakeDamage(damage, transform, knockbackRange, attackStatus, statusChance, statusDuration);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (damageType != DamageType.PerSecond) return;

        if (other.transform.name == "Hitbox" && other.transform.root.GetComponent<PlayerController>() != null)
        {
            Debug.Log("AOE hitbox overlap detected");

            if (Time.time - lastDamageTime >= damageInterval)
            {
                lastDamageTime = Time.time;

                if (PlayerCombat.instance != null)
                {
                    Debug.Log("AOE damage applied");
                    PlayerCombat.instance.TakeDamage(damage, transform, knockbackRange, attackStatus, statusChance, statusDuration);
                }
            }
        }
    }

}
