using UnityEngine;
using System.Collections;

public class Enemy_Pathfinding : MonoBehaviour
{
    private float moveSpeed;
    public bool hidingMode { get; private set; }

    private bool flipStartsFacingRight;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private KnockbackEffect knockback;
    private Enemy_Health enemyHealth;
    private Enemy_Config config;

    public void stopHide()
    {
        hidingMode = false;
    }

    private void Awake()
    {
        knockback = GetComponent<KnockbackEffect>();
        config = GetComponent<Enemy_Config>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<Enemy_Health>();
        hidingMode = config.HidingMode;
        moveSpeed = config.MoveSpeed;
        flipStartsFacingRight = config.SpriteFacingRight;
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
