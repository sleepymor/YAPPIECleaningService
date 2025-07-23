using UnityEngine;

public enum DamageType { OnHit, PerSecond }

public class Enemy_Projectiles : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] public int damage = 1;
    [SerializeField] private DamageType damageType = DamageType.PerSecond;
    [SerializeField] private float damageInterval = 10f; 

    [Header("Movement Settings")]
    [SerializeField] public float speed = 10f;
    [SerializeField] public float maxTravelDistance = 15f;

    [SerializeField] private float aoeLifetime = 7f;  // Add this field
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
        lastDamageTime = Time.time - damageInterval; // allow immediate tick
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

        if (other.name == "Hitbox" && PlayerCombat.instance != null)
        {
            PlayerCombat.instance.TakeDamage(damage, transform);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (damageType != DamageType.PerSecond) return;

        if (other.name == "Hitbox")
        {
            Debug.Log("AOE hitbox overlap detected");

            if (Time.time - lastDamageTime >= damageInterval)
            {
                lastDamageTime = Time.time;

                if (PlayerCombat.instance != null)
                {
                    Debug.Log("AOE damage applied");
                    PlayerCombat.instance.TakeDamage(damage, transform);
                }
            }
        }
    }

}
