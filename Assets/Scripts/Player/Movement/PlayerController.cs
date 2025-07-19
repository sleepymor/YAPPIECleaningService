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

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Move();
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
        }
    }


    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));

        if (movement != Vector2.zero)
        {
            _animator.SetBool("isRunning", true);
            _animator.SetFloat("moveX", movement.x);
            _animator.SetFloat("moveY", movement.y);
            spriteRenderer.flipX = movement.x < 0;
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }
    }

}
