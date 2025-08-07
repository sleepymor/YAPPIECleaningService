using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private PlayerConfig config;
    public Vector2 LastMoveDirection { get; private set; }
    public bool canMove = true;

    private float moveSpeed;
    private SpriteRenderer spriteRenderer;
    private Animator _animator;
    private Rigidbody2D rb;

    private float dashSpeed;
    private float dashDuration;
    private float dashCooldown;

    private bool isDashing = false;
    private bool canDash = true;

    private float lastFacingX = 1f; 

    public Controller controller;
    public Vector2 movement { get; set;}

    private void Awake()
    {
        transform.SetParent(null); // Detach from parent
        instance = this;
        config = GetComponent<PlayerConfig>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        controller = new Controller();
        moveSpeed = config.MoveSpeed;
        dashSpeed = config.DashSpeed;
        dashCooldown = config.DashCooldown;
        dashDuration = config.DashDuration;

    }

    void Start()
    {
        MissionManager.instance.DefineAllMissions();
        MissionManager.instance.ActivateMission("Slay 5 Oil Drop");

    }

    private void OnEnable()
    {
        controller.Enable();
    }

    private void OnDisable()
    {
        controller.Disable();
    }

    private void Update()
    {
        if (PlayerCombat.instance.isStunned == false)
        {
            PlayerInput();
            if (controller.Movement.Dash.triggered && canDash && !isDashing)
            {
                StartCoroutine(Dash());
            }
        } else
        {
            rb.MovePosition(rb.position + movement * (0 * Time.fixedDeltaTime));
        }

        DataManager.instance.playerPos = transform.position;
    }

    private void FixedUpdate()
    {
        Move();
    }


    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        canMove = false;

        Vector2 dashDir = movement != Vector2.zero ? movement.normalized : LastMoveDirection;

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.MovePosition(rb.position + dashDir * (dashSpeed * Time.fixedDeltaTime));
            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        canMove = true;
        SkillUI.instance.TriggerDashCooldown();
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }


    private void PlayerInput()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            return;
        }

        movement = controller.Movement.Move.ReadValue<Vector2>();

        if (movement != Vector2.zero)
        {
            LastMoveDirection = movement.normalized;

            if (movement.x != 0)
            {
                lastFacingX = movement.x;
            }
        }
    }



    private void Move()
    {
        if (!canMove)
            return;

        KnockbackEffect knockback = GetComponent<KnockbackEffect>();
        if (knockback != null && knockback.gettingKnockedBack)
            return;

        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));

        if (movement != Vector2.zero)
        {
            _animator.SetBool("isRunning", true);
            _animator.SetFloat("moveX", movement.x);
            _animator.SetFloat("moveY", movement.y);

            if (movement.x != 0)
            {
                spriteRenderer.flipX = movement.x < 0;
            }
            else
            {
                spriteRenderer.flipX = lastFacingX < 0;
            }
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }
    }

}
