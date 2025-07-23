using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;


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
    [SerializeField] public Transform harpoonPullArea;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] public float harpoonRange = 4f;
    [SerializeField] private GameObject harpoonAimPrefab;
    
    private GameObject harpoonAimInstance;

    private float holdTime = 0f;
    private bool isHolding = false;
    private bool hasAttacked = false;
    private float requiredHoldTime = 0.5f;
    private bool isInvincible = false;

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
        harpoonPullArea.gameObject.SetActive(false);

    }

    void Start()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        hitDmgArea.gameObject.SetActive(false);
        harpoonPullArea.gameObject.SetActive(false);
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
        float input = combat.Fight.Attack.ReadValue<float>();

        if (input > 0)
        {
            if (!isHolding)
            {
                isHolding = true;
                holdTime = 0f;
                hasAttacked = false;
                Debug.Log("Mulai menahan tombol...");
            }

            holdTime += Time.deltaTime;

            if (holdTime >= requiredHoldTime)
            {
                if (harpoonAimInstance == null)
                {
                    harpoonAimInstance = Instantiate(harpoonAimPrefab);
                    Debug.Log("Harpoon Aim muncul");
                }

                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0f;
                harpoonAimInstance.transform.position = mouseWorld;
            }
            if (holdTime < requiredHoldTime && !hasAttacked)
            {
                hasAttacked = true; // Prevent repeated triggering
                isAttacking = true;
                Vector2 currentDir = PlayerController.instance.LastMoveDirection;
                UpdateDirection(currentDir);
                StartCoroutine(AttackMove());
            }

        }

        if (combat.Fight.Attack.WasReleasedThisFrame())
        {
            if (harpoonAimInstance != null && holdTime >= requiredHoldTime)
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0f;

                Vector2 direction = (mouseWorld - transform.position).normalized;
                UpdateHarpoonDirection(direction);

                harpoonPullArea.gameObject.SetActive(true);
                StartCoroutine(DisableHarpoonPullArea());

                spriteRenderer.flipX = direction.x < 0f;

                StartCoroutine(TemporarilyDisableMovement(0.3f));
            }

            if (harpoonAimInstance != null)
            {
                Destroy(harpoonAimInstance);
                Debug.Log("Harpoon Aim dihancurkan");
            }

            isHolding = false;
            holdTime = 0f;
            hasAttacked = false;
        }

    }

    IEnumerator TemporarilyDisableMovement(float duration)
    {
        PlayerController.instance.canMove = false;
        yield return new WaitForSeconds(duration);
        PlayerController.instance.canMove = true;
    }


    void UpdateHarpoonDirection(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float offset = 1f;

        Vector3 newPosition = new Vector3(dir.x, dir.y, 0) * offset;

        harpoonPullArea.localRotation = Quaternion.Euler(0, 0, angle);
        harpoonPullArea.localPosition = newPosition;
    }

    IEnumerator DisableHarpoonPullArea()
    {
        yield return new WaitForSeconds(0.2f); // Adjust time as needed
        harpoonPullArea.gameObject.SetActive(false);
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



    public void TakeDamage(int damage, Transform attacker)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        knockback.GetKnockedBack(attacker, 15f);
        animator.SetBool("isHit", true);

        StartCoroutine(InvincibilityCoroutine());

        CheckDeath();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float flashDuration = 0.1f;
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashDuration);
            elapsed += flashDuration * 2;
        }

        isInvincible = false;
    }




    public void CheckDeath()
    {
        if(currentHealth < 1)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
