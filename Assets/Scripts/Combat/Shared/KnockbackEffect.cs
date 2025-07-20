using UnityEngine;
using System.Collections;

public class KnockbackEffect : MonoBehaviour
{
    public bool gettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = 0.2f;
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
        gettingKnockedBack = true;

        if (isPlayer)
        {
            // Disable player movement
            PlayerController.instance.canMove = false;
        }

        Vector2 difference = (transform.position - damageSource.position).normalized * knockBackThrust * rb.mass;
        rb.AddForce(difference, ForceMode2D.Impulse);

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
        {
            PlayerController.instance.canMove = true;
        }

        onKnockbackEnd();
    }
}
