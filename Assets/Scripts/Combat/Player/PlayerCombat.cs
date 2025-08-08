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
    public Rigidbody2D rb { get; set; }

    public float attackMoveSpeed;
    public float attackMoveDuration;

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
    private Vector2 lastHarpoonDirection = Vector2.right;


    [HideInInspector]
    public bool isAttacking;
    private Vector2 lastMoveDirection = Vector2.right;

    private LineRenderer harpoonLine;
    private bool isCharge = false;


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
            Destroy(gameObject);
            return;
        }
        instance = this;

        harpoonLine = gameObject.AddComponent<LineRenderer>();
        harpoonLine.startWidth = 0.05f;
        harpoonLine.endWidth = 0.05f;
        harpoonLine.sortingOrder = 9;
        Color brown = new Color(0.59f, 0.29f, 0.0f); // RGB(150, 75, 0)
        harpoonLine.material = new Material(Shader.Find("Sprites/Default"));
        harpoonLine.startColor = brown;
        harpoonLine.endColor = brown;
        harpoonLine.positionCount = 0;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        hitDmgArea.gameObject.SetActive(false);
        harpoonPullArea.gameObject.SetActive(false);

        Initialize();
        MissionManager.instance.ReloadActiveMissionsFromDataManager();
    }

    public Transform defaultSpawn;
    public Transform areaSpawn;


    public void Initialize()
    {
        if (DataManager.instance.PlayerHealth == -99999999)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth = DataManager.instance.PlayerHealth;
        }

        Vector2 savedPos = DataManager.instance.savedPlayerPos;

        if (savedPos == Vector2.zero)
        {
            savedPos = defaultSpawn.position;
        }

        if (DataManager.instance.playerScene == "")
        {
            savedPos = areaSpawn.position;
        }

        rb.position = savedPos;
        transform.position = savedPos;

        Debug.Log("Initialized at position: " + savedPos);
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

        UpdateHarpoonLine();

    }

    void UpdateHarpoonLine()
    {
        if (harpoonTipInstance != null)
        {
            harpoonLine.positionCount = 2;
            harpoonLine.SetPosition(0, transform.position);
            harpoonLine.SetPosition(1, harpoonTipInstance.transform.position);
        }
        else
        {
            harpoonLine.positionCount = 0;
        }
    }


    private int currentCombo = 0;
    public string harpoonMission;
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
                //StartCoroutine(AttackMove());
                if(currentCombo == 0)
                {
                    Attack1();
                } else if (currentCombo == 1) {
                    Attack2();
                } else
                {
                    Attack3();
                    currentCombo = 0;
                }
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
                ShootHarpoon(lastHarpoonDirection);
                SkillUI.instance.TriggerHarpoonCooldown();
                if (harpoonMission != null)
                {
                    MissionManager.instance.ForceCompleteMission(harpoonMission);
                }
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

        lastHarpoonDirection = dir.normalized;
    }

    IEnumerator DisableHarpoonPullArea()
    {
        yield return new WaitForSeconds(0.2f);
        harpoonPullArea.gameObject.SetActive(false);
    }

    public GameObject harpoonTipPrefab;


    private GameObject harpoonTipInstance;

    private void ShootHarpoon(Vector2 direction)
    {
        if (harpoonTipPrefab == null) return;

        direction = direction.normalized;
        Vector2 spawnPos = (Vector2)transform.position + direction * 0.5f;

        GameObject proj = Instantiate(harpoonTipPrefab, spawnPos, Quaternion.identity);
        harpoonTipInstance = proj;

        Harpoon_Tip projectile = proj.GetComponent<Harpoon_Tip>();
        if (projectile != null)
        {
            projectile.damage = 1;
            projectile.maxTravelDistance = 10f;
            projectile.speed = 50f;
            projectile.playerPos = transform;
            projectile.Initialize(direction);
        }
    }



    public void OnAttackSuccess()
    {
        hitDmgArea.gameObject.SetActive(true);
    }

    void UpdateDirection(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        lastMoveDirection = dir.normalized;
        UpdateHitDmgAreaDirection();
    }

    private bool isCombo = false;

    public void Attack1()
    {
        PlayerController.instance.canMove = false;
        isCombo = true;
        animator.SetBool("isCombo", true);
        animator.SetBool("attack1", true);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        currentCombo = 1;
        isCharge = true;
    }

    public void OnAttackEnds()
    {
        isCombo = false;
        isCharge = false;
        currentCombo = 0;
        animator.SetBool("isCombo", false);
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);

    }

    public void OnTransitionToAttack2()
    {
        PlayerController.instance.canMove = true;
        isCharge = false;
    }

    public void Attack2()
    {
        PlayerController.instance.canMove = false;
        isCombo = true;
        isCharge = true;
        animator.SetBool("attack2", true);
        animator.SetBool("attack1", true);
        animator.SetBool("attack3", false);
        currentCombo = 2;
    }

    public void OnTransitionToAttack3()
    {
        PlayerController.instance.canMove = true; 
        isCharge = false;

    }

    public void Attack3()
    {
        PlayerController.instance.canMove = false;
        isCombo = true;
        isCharge = true;
        animator.SetBool("attack3", true);
        animator.SetBool("attack2", true);
        animator.SetBool("attack1", true);
        currentCombo = 0;
    }

    public void ChargeFront()
    {
        rb.AddForce(lastMoveDirection * attackMoveSpeed, ForceMode2D.Impulse);
    }

    public void OnFinalTransition()
    {
        PlayerController.instance.canMove = true;
        isCharge = false;
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

    public void ClearHarpoonReference()
    {
        harpoonTipInstance = null;
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
            CleanupDontDestroyOnLoad();
            SceneManager.LoadScene("GameOver");
        }
    }

    void CleanupDontDestroyOnLoad()
    {
        GameObject temp = new GameObject("TempCleanup");
        DontDestroyOnLoad(temp);

        Scene dontDestroyScene = temp.scene;

        Destroy(temp);

        // Destroy all root GameObjects in the DontDestroyOnLoad scene
        foreach (GameObject obj in dontDestroyScene.GetRootGameObjects())
        {
            Destroy(obj);
        }
    }

}
