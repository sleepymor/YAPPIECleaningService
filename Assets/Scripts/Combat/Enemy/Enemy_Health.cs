using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int numberOfFlashes = 5;

    public static event System.Action<Enemy_Health> OnEnemyDeath;

    public bool IsDead { get; private set; } = false;

    private int currentHealth;
    private KnockbackEffect knockback;
    private Animator animator;
    private Color originalColor;

    private void Awake()
    {
        knockback = GetComponent<KnockbackEffect>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        Debug.Log(currentHealth);

        knockback.GetKnockedBack(PlayerController.instance.transform, 15f);
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0 && !IsDead)
        {
            IsDead = true;
            animator.SetBool("isDeath", true);
            StartCoroutine(FlashBlink(numberOfFlashes)); // Only on death
            OnEnemyDeath?.Invoke(this);
        }
    }

    public void ClearDeath()
    {
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator FlashBlink(int times)
    {
        for (int i = 0; i < times; i++)
        {
            // Invisible
            Color transparentColor = originalColor;
            transparentColor.a = 0f;
            spriteRenderer.color = transparentColor;

            yield return new WaitForSeconds(flashDuration);

            // Visible
            spriteRenderer.color = originalColor;

            yield return new WaitForSeconds(flashDuration);
        }
    }
}
