using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Pathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private KnockbackEffect knockback;


    private void Awake()
    {
        knockback = GetComponent<KnockbackEffect>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (knockback.gettingKnockedBack) { return; }
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
        if (moveDir != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
            spriteRenderer.flipX = moveDir.x < 0;
        } else
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = targetPosition;
    }
}
