using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator _animator;
    private Controller controller;
    private Vector2 movement;
    private Rigidbody2D rb;

    private void Awake()
    {
        controller = new Controller();
        rb = GetComponent<Rigidbody2D>();

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
        movement = controller.Movement.Move.ReadValue<Vector2>();
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
