using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    [SerializeField] public int damage;
    public static Enemy_Attack instance;
    private Animator animator;

    private void Awake()
    {
        instance = this;
    }

    public void OnDamageSent()
    {
        if (Enemy_Detection.instance == null || Enemy_Detection.instance.currentTarget == null)
        {
            Debug.LogWarning("Enemy detection or current target is missing.");
            return;
        }

        if (PlayerCombat.instance == null)
        {
            Debug.LogWarning("PlayerCombat instance is missing.");
            return;
        }

        Vector2 start = transform.position;
        Vector2 end = Enemy_Detection.instance.currentTarget.position;
        float distanceToTarget = Vector2.Distance(start, end);

        if (distanceToTarget <= Enemy_Detection.instance.attackRange)
        {
            PlayerCombat.instance.TakeDamage(damage);
        }
    }


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        animator.SetBool("attack", true);
    }

    public void StopAttack()
    {
        animator.SetBool("attack", false);
    }
}
