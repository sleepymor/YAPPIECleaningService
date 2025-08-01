using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    private int startingHealth;
    private SpriteRenderer spriteRenderer;
    private float flashDuration;
    private int numberOfFlashes;
    private string killMissionName;

    public static event System.Action<Enemy_Health> OnEnemyDeath;

    private int enemyID;


    public bool IsDead { get; private set; } = false;

    private int currentHealth;
    private KnockbackEffect knockback;
    private Animator animator;
    private Color originalColor;
    private EnemyEnvironment environment;

    Enemy_Config config;

    private void Awake()
    {
        config = GetComponent<Enemy_Config>();
        startingHealth = config.MaximumHealth;
        flashDuration = config.FlashDuration;
        numberOfFlashes = config.NumberOfFlashes;
        environment = config.Environment;
        killMissionName = config.KillMissionName;
        knockback = GetComponent<KnockbackEffect>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyID = DataManager.instance.RegisterEnemyHealth(startingHealth);
    }

    private void Start()
    {
        int savedHealth = DataManager.instance.GetEnemyHealth(enemyID);
        currentHealth = savedHealth != -1 ? savedHealth : startingHealth;
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        EnvironmentManager.Instance.RegisterEnemy(environment);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        Debug.Log(currentHealth);
        DataManager.instance.UpdateEnemyHealth(enemyID, currentHealth);

        knockback.GetKnockedBack(PlayerController.instance.transform, 15f);
        DetectDeath();
    }



    public void PullDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        Debug.Log(currentHealth);

        DataManager.instance.UpdateEnemyHealth(enemyID, currentHealth);

        knockback.PullToward(PlayerController.instance.transform, 80f);
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
        EnvironmentManager.Instance.EnemyKilled(environment);
        if (!string.IsNullOrEmpty(killMissionName))
        {
            MissionManager.instance.UpdateEnemyKill(killMissionName);
        }
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
