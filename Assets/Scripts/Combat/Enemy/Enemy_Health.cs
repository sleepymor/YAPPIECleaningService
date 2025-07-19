using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;

    private int currentHealth;
    private KnockbackEffect knockback;
    private Animator animator;

    private void Awake()
    {
        knockback = GetComponent<KnockbackEffect>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        knockback.GetKnockedBack(PlayerController.instance.transform, 15f);
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            animator.SetBool("isDeath", true);
        }
    }

    public void ClearDeath()
    {
        Destroy(gameObject);
    }

}
