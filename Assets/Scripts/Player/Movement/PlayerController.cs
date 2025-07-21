using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Vector2 LastMoveDirection { get; private set; }
    public bool canMove = true;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;

    private bool isDashing = false;
    private bool canDash = true;

    private float lastFacingX = 1f; 

    private Controller controller;
    public Vector2 movement;

    private void Awake()
    {
        instance = this;
        controller = new Controller();
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
        PlayerInput();
        if (controller.Movement.Dash.triggered && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
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

        // Start cooldown timer
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
