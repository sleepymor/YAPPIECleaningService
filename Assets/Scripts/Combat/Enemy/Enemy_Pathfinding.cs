using UnityEngine;
using System.Collections;

public class Enemy_Pathfinding : MonoBehaviour
{
    public float moveSpeed;
    public bool hidingMode;

    private bool flipStartsFacingRight;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private KnockbackEffect knockback;
    private Enemy_Health enemyHealth;
    private Enemy_Config config;

    // Internal speed multiplier
    private float speedMultiplier = 1f;

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
            rb.MovePosition(rb.position + moveDir * (moveSpeed * speedMultiplier * Time.fixedDeltaTime));
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

    // Default speedMultiplier = 1 (normal speed)
    public void MoveTo(Vector2 targetDirection, float speedMultiplier = 1f)
    {
        this.moveDir = targetDirection;
        this.speedMultiplier = speedMultiplier;
    }
}
