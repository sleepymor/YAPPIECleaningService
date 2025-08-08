using UnityEngine;

public class Enemy_Detection : MonoBehaviour
{
    public Transform detectionArea { get; set; }
    public bool isDetecting;

    public float meleeRange { get; set; }
    public float rangedRange { get; set; }
    public bool useTrigger;
    public bool isBoss;
    public float triggerRange;

    [HideInInspector] public Transform currentTarget;

    private Animator animator;
    private Enemy_Pathfinding pathfinding;
    private Enemy_Attack enemyAttack;
    private Enemy_Config config;

    private Enemy_Boss enemyBoss;

    private void Awake()
    {
        isDetecting = false;
        config = GetComponent<Enemy_Config>();
        animator = GetComponent<Animator>();
        pathfinding = GetComponent<Enemy_Pathfinding>();
        enemyAttack = GetComponent<Enemy_Attack>();

        useTrigger = config.UseTrigger;
        triggerRange = config.TriggerRange;
        meleeRange = config.MeleeRange;
        rangedRange = config.RangedRange;
        detectionArea = config.DetectionArea;
        isBoss = config.IsBoss;

        if (isBoss)
        {
            enemyBoss = GetComponent<Enemy_Boss>();
        }

        if (detectionArea != null)
            detectionArea.gameObject.SetActive(true);
    }

    private float GetEffectiveRange()
    {
        if (enemyAttack == null) return meleeRange;

        if (enemyAttack.CurrentAttackMode == AttackMode.AOE)
        {
            if (enemyAttack.IsAOEReady())
                return rangedRange;
            else
                return meleeRange;
        }

        return enemyAttack.CurrentAttackMode == AttackMode.Ranged ? rangedRange : meleeRange;
    }

    private void FixedUpdate()
    {
        if (isDetecting && currentTarget != null)
        {
            Vector2 start = transform.position;
            Vector2 end = currentTarget.position;

            Vector2 direction = (end - start).normalized;
            float distanceToTarget = Vector2.Distance(start, end);
            float effectiveRange = GetEffectiveRange();

            if (distanceToTarget <= effectiveRange)
            {
                if (HasBoolParam(animator, "isHiding") && useTrigger && distanceToTarget <= triggerRange)
                {
                    animator.SetBool("isHiding", false);
                }

                pathfinding.MoveTo(Vector2.zero); // stop moving
                enemyAttack.Attack();
            }
            else
            {
                enemyAttack.StopAttack();
                pathfinding.MoveTo(direction); // move towards player
            }
        }
        else
        {
            // No target detected: stop movement and attack
            pathfinding.MoveTo(Vector2.zero);
            enemyAttack.StopAttack();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentTarget = other.transform;
            isDetecting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentTarget = null;
            isDetecting = false;

            pathfinding.MoveTo(Vector2.zero);
            enemyAttack.StopAttack();
        }
    }

    private bool HasBoolParam(Animator anim, string paramName)
    {
        if (anim == null) return false;

        foreach (var param in anim.parameters)
        {
            if (param.name == paramName && param.type == AnimatorControllerParameterType.Bool)
                return true;
        }
        return false;
    }
}
