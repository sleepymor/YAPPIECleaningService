using UnityEngine;
using System.Collections;

public class KnockbackEffect : MonoBehaviour
{
    public bool gettingKnockedBack;

    private float knockBackTime = 0.2f;
    [SerializeField] private bool isPlayer = false;

    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        ApplyForceFromSource(damageSource, knockBackThrust, pushAway: true);
    }

    public void PullToward(Transform pullSource, float pullStrength)
    {
        ApplyForceFromSource(pullSource, pullStrength, pushAway: false);
    }

    private void ApplyForceFromSource(Transform source, float thrust, bool pushAway)
    {
        gettingKnockedBack = true;

        if (isPlayer)
            PlayerController.instance.canMove = false;

        Vector2 direction = (transform.position - source.position).normalized;
        if (!pushAway)
            direction *= -1f;

        Vector2 force = direction * thrust * rb.mass;
        rb.AddForce(force, ForceMode2D.Impulse);

        animator.SetBool("isHit", true);
        StartCoroutine(KnockRoutine());
    }

    public void onKnockbackEnd()
    {
        animator.SetBool("isHit", false);
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);

        rb.linearVelocity = Vector2.zero;
        gettingKnockedBack = false;

        if (isPlayer)
            PlayerController.instance.canMove = true;

        onKnockbackEnd();
    }
}
