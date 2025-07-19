using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public Animator animator;
    [SerializeField] private float attackMoveSpeed = 3f;
    [SerializeField] private float attackMoveDuration = 0.2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public Transform hitDmgArea;

    private Combat combat;
    public bool isAttacking = false;
    private Vector2 lastMoveDirection = Vector2.right;

    public void Awake()
    {
        instance = this;
        combat = new Combat();
        rb = GetComponent<Rigidbody2D>();
        hitDmgArea.gameObject.SetActive(false);
    }

    void Start()
    {
        animator = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        combat.Enable();
    }

    private void OnDisable()
    {
        combat.Disable();
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

        while (timer < attackMoveDuration)
        {
            rb.MovePosition(rb.position + dir * (attackMoveSpeed * Time.fixedDeltaTime));
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

        // Optional offset for placing the hit area ahead of the player
        float offset = 1f;
        Vector3 newPosition = new Vector3(dir.x, dir.y, 0) * offset;

        hitDmgArea.localRotation = Quaternion.Euler(0, 0, angle);
        hitDmgArea.localPosition = newPosition;
    }
}
