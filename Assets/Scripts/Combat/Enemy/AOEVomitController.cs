using UnityEngine;

public class AoeVomitController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int projectileCount = 10;
    [SerializeField] private float spreadAngle = 25f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private int damage = 1;

    private void Start()
    {
        FireVomitProjectiles();
    }

    private void FireVomitProjectiles()
    {
        float halfSpread = spreadAngle / 2f;
        float baseAngle = transform.eulerAngles.z;

        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = Mathf.Lerp(-halfSpread, halfSpread, (float)i / (projectileCount - 1));
            float angle = baseAngle + angleOffset;

            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            Vector2 spawnPos = (Vector2)transform.position + direction * 0.2f;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Enemy_Projectiles p = proj.GetComponent<Enemy_Projectiles>();
            if (p != null)
            {
                p.damage = damage;
                p.Initialize(direction * projectileSpeed);
            }
        }
    }
}
