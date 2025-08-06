using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{
    private Animator animator;
    private Enemy_Config config;

    void Awake()
    {
        config = GetComponent<Enemy_Config>();
        animator = GetComponent<Animator>();
    }
    public void randomAttack()
    {
        stopAttack();
        float rand = Random.Range(0f, 100f);

        if (rand < 10f)
        {
            powerStompAttack();
        }
        else if (rand < 40f)
        {
            stompAttack();
        }
        else if (rand < 60f)
        {
            crushAttack();
        }
        else
        {
            meleeAttack();
        }
    }

    public void stopAttack()
    {
        if (animator) animator.SetBool("stompAttack", false);
        if (animator) animator.SetBool("powerStompAttack", false);
        if (animator) animator.SetBool("meleeAttack", false);
        if (animator) animator.SetBool("crushAttack", false);

    }

    private void stompAttack()
    {
        if (animator) animator.SetBool("stompAttack", true);
    }

    private void powerStompAttack()
    {
        if (animator) animator.SetBool("powerStompAttack", true);
    }

    private void meleeAttack()
    {
        if (animator) animator.SetBool("meleeAttack", true);
    }

    private void crushAttack()
    {
        if (animator) animator.SetBool("crushAttack", true);
    }
}
