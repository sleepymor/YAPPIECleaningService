using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;

    public int startingHealth { get; set; }
    public int currentHealth { get; set; }
    public bool isStunned { get; set; }

    public Animator animator { get; set; }
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private float attackMoveSpeed;
    private float attackMoveDuration;

    public Transform hitDmgArea { get; set; }
    public Transform harpoonPullArea { get; set; }

    private float invincibilityDuration;
    private float flashDuration;

    private float harpoonRange;

    private GameObject harpoonAimPrefab;
    private GameObject harpoonAimInstance;
    private float harpoonCooldown;
    private float lastHarpoonTime = -Mathf.Infinity;

    private float holdTime = 0f;
    private bool isHolding = false;
    private bool hasAttacked = false;
    private float requiredHoldTime = 0.5f;
    private bool isInvincible = false;

    private KnockbackEffect knockback;

    private PlayerConfig config;

    private Combat combat;

    [HideInInspector]
    public bool isAttacking;
    private Vector2 lastMoveDirection = Vector2.right;

    public void Awake()
    {
        isStunned = false;
        
        combat = new Combat();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        config = GetComponent<PlayerConfig>();
        knockback = GetComponent<KnockbackEffect>();

        startingHealth = config.MaximumHealth;
        hitDmgArea = config.MeleeDmgArea;

        harpoonPullArea = config.HarpoonDmgArea;
        harpoonRange = config.HarpoonRange;
        harpoonAimPrefab = config.HarpoonAimPrefab;
        harpoonCooldown = config.HarpoonCooldown;

        attackMoveSpeed = config.MeleeAttackMoveSpeed;
        attackMoveDuration = config.MeleeAttackMoveDuration;

        invincibilityDuration = config.InvincibilityDuration;
        flashDuration = config.InvincibilityFlashDuration;

        hitDmgArea.gameObject.SetActive(false);

        harpoonPullArea.gameObject.SetActive(false);

        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }
        instance = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        hitDmgArea.gameObject.SetActive(false);
        harpoonPullArea.gameObject.SetActive(false);

        if (DataManager.instance.PlayerHealth == -99999999)
        {
            currentHealth = startingHealth;
        } else
        {
            currentHealth = DataManager.instance.PlayerHealth;
        }
    }

    void OnDestroy()
    {
        if (DataManager.instance != null)
        {
            DataManager.instance.PlayerHealth = currentHealth;
        }
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
        if (!isStunned)
        {
            Attack();
        }
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

            if (holdTime >= requiredHoldTime && Time.time >= lastHarpoonTime + harpoonCooldown)
            {
                if (harpoonAimInstance == null)
                {
                    harpoonAimInstance = Instantiate(harpoonAimPrefab);
                    SkillUI.instance.TriggerHarpoonCooldown();
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
                lastHarpoonTime = Time.time; // Start cooldown timer
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
        yield return new WaitForSeconds(0.2f);
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



    public void TakeDamage(int damage, Transform attacker, float knockbackRange, AttackStatus statusEffect, float statusChance, float statusTime)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        knockback.GetKnockedBack(attacker, 15f);
        animator.SetBool("isHit", true);

        if (statusEffect == AttackStatus.Stun && Random.value <= statusChance)
        {
            StartCoroutine(ApplyStun(statusTime));
        }

        StartCoroutine(InvincibilityCoroutine());
        CheckDeath();
    }

    private IEnumerator ApplyStun(float duration)
    {
        isStunned = true;
        animator.SetBool("isStunned", true); // Optional: add animation feedback
        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);


        yield return new WaitForSeconds(duration);
        spriteRenderer.color = Color.white;
        isStunned = false;
        animator.SetBool("isStunned", false); // Optional: reset animation
    }


    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

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


    public void ChargeFull()
    {
        currentHealth = startingHealth;
        StartCoroutine(InvincibilityCoroutine());
    }

    public void CheckDeath()
    {
        if(currentHealth < 1)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
