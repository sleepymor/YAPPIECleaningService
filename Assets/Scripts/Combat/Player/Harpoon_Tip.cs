using UnityEngine;

public class Harpoon_Tip : MonoBehaviour
{
    [Header("Movement Settings")]
    public int damage { get; set; }
    public float speed { get; set; }
    public float maxTravelDistance { get; set; }
    public Transform playerPos { get; set; }

    private Vector2 direction;
    private Vector2 startPosition;
    private bool isGoback = false;
    private bool hasHit = false;

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        startPosition = transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Debug.Log("Spawned Harpoon");
    }

    private void Update()
    {
        if (!isGoback)
        {
            // Go forward
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector2.Distance(startPosition, transform.position) >= maxTravelDistance)
            {
                isGoback = true;
            }
        }
        else
        {
            if (playerPos == null) return;

            // Chase player
            Vector2 toPlayer = ((Vector2)playerPos.position - (Vector2)transform.position).normalized;
            transform.Translate(toPlayer * speed * Time.deltaTime, Space.World);

            // Rotate to face player
            float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 180f);

            if (Vector2.Distance(playerPos.position, transform.position) < 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.TryGetComponent(out Enemy_Health enemyHealth))
        {
            hasHit = true;
            StartCoroutine(HitPauseThenReturn(enemyHealth));
        }
    }

    private System.Collections.IEnumerator HitPauseThenReturn(Enemy_Health enemyHealth)
    {
        float originalSpeed = speed;
        speed = 0f; // Stop movement

        yield return new WaitForSeconds(0.2f); // Pause on hit

        enemyHealth.PullDamage(damage); // Apply damage
        isGoback = true; // Start returning
        speed = originalSpeed; // Resume speed
    }


    private void OnDestroy()
    {
        if (PlayerCombat.instance != null)
        {
            PlayerCombat.instance.ClearHarpoonReference();
        }
    }


}
