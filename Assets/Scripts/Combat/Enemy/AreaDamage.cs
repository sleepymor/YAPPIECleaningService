using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaDamage : MonoBehaviour
{
    public int damageAmount = 1;
    private Collider2D areaCollider;

    public int damage;
    public float knockbackRange;

    public AttackStatus aoeAttackStatus;
    public float aoeAttackStatusChance;
    public float aoeAttackStatusDuration;
    public bool isPlayerInside = false;

    private Animator animator;

    private void Awake()
    {
        areaCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        areaCollider.isTrigger = true;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private void OnEnable()
    {
        isPlayerInside = false;
        animator.Play("mutantlandfill_area_anim");
            }

}
