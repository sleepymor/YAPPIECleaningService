using UnityEngine;
using System.Collections;

public class Enemy_Pathfinding : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] public bool hidingMode = false;

    [Header("Sprite Orientation")]
    [SerializeField] private bool flipStartsFacingRight = true;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private KnockbackEffect knockback;
    private Enemy_Health enemyHealth;

    public void stopHide()
    {
        hidingMode = false;
    }

    private void Awake()
    {
        knockback = GetComponent<KnockbackEffect>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<Enemy_Health>();
    }

    private void FixedUpdate()
    {
        if (knockback.gettingKnockedBack) return;

        if (!enemyHealth.IsDead && !hidingMode)
        {
            rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
        }

        if (moveDir != Vector2.zero)
        {
            animator.SetBool("isMoving", true);

            spriteRenderer.flipX = flipStartsFacingRight ? moveDir.x < 0 : moveDir.x > 0;
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = targetPosition;
    }
}
