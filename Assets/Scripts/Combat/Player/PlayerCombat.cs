using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;

    [SerializeField] public int startingHealth = 10;

    public int currentHealth;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public Animator animator;
    [SerializeField] private float attackMoveSpeed = 3f;
    [SerializeField] private float attackMoveDuration = 0.2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public Transform hitDmgArea;
    private KnockbackEffect knockback;

    private Combat combat;
    public bool isAttacking = false;
    private Vector2 lastMoveDirection = Vector2.right;

    public void Awake()
    {
        instance = this;
        combat = new Combat();
        rb = GetComponent<Rigidbody2D>(); 
        knockback = GetComponent<KnockbackEffect>();
        hitDmgArea.gameObject.SetActive(false);
    }

    void Start()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        hitDmgArea.gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        combat.Enable();
    }

    private void OnDisable()
    {
        combat.Disable();
    }

    public void DisableAttack()
    {
        hitDmgArea.gameObject.SetActive(false);
    }

    void Update()
    {
        Attack();

    }

    void Attack()
    {
        if (combat.Fight.Attack.triggered && !isAttacking)
        {
            isAttacking = true;

            Vector2 currentDir = PlayerController.instance.LastMoveDirection;
            UpdateDirection(currentDir);

            StartCoroutine(AttackMove());
        }
    }

    public void OnAttackSuccess()
    {
        hitDmgArea.gameObject.SetActive(true);
    }

    System.Collections.IEnumerator AttackMove()
    {
        PlayerController.instance.canMove = false;

        float timer = 0f;
        Vector2 dir = PlayerController.instance.LastMoveDirection;

        while (timer < attackMoveDuration/2)
        {
            rb.MovePosition(rb.position + dir * (attackMoveSpeed * Time.fixedDeltaTime));
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < attackMoveDuration*2)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        hitDmgArea.gameObject.SetActive(false);
        PlayerController.instance.canMove = true;
    }

    void UpdateDirection(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        lastMoveDirection = dir.normalized;
        UpdateHitDmgAreaDirection();
    }

    void UpdateHitDmgAreaDirection()
    {
        Vector2 dir = lastMoveDirection;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float offset = 1f;
        Vector3 newPosition = new Vector3(dir.x, dir.y, 0) * offset;

        hitDmgArea.localRotation = Quaternion.Euler(0, 0, angle);
        hitDmgArea.localPosition = newPosition;
    }



    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        knockback.GetKnockedBack(Enemy_Attack.instance.transform, 15f);
        animator.SetBool("isHit", true);
        CheckDeath();
    }

    public void CheckDeath()
    {
        if(currentHealth < 1)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
