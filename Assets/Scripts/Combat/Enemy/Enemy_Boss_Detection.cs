using UnityEngine;

public class Enemy_Boss_Detection : MonoBehaviour
{
    public Transform detectionArea { get; set; }
    public bool isDetecting { get; set; }
    private LayerMask obstacleMask;

    public float meleeRange = 2f;
    public float rangedRange = 6f;
    public float aoeRange = 4f;
    public float dashRange = 8f;

    [HideInInspector] public Transform currentTarget;
    [HideInInspector] public bool hasLineOfSight;

    private Animator animator;
    private Enemy_Pathfinding pathfinding;
    private Enemy_Attack enemyAttack;
    private Enemy_Config config;
    private Enemy_Boss enemyBoss;

    private float attackCooldown = 2f;
    private float lastAttackTime = -999f;

    private enum AttackType { Melee, AOE, Ranged, Dash }
    private AttackType currentAttack;

    private void Start()
    {
        isDetecting = false;
        config = GetComponent<Enemy_Config>();
        animator = GetComponent<Animator>();
        pathfinding = GetComponent<Enemy_Pathfinding>();
        enemyAttack = GetComponent<Enemy_Attack>();
        enemyBoss = GetComponent<Enemy_Boss>();

        meleeRange = config.MeleeRange;
        rangedRange = config.RangedRange;
        detectionArea = config.DetectionArea;
        obstacleMask = config.ObstacleMask;

        detectionArea.gameObject.SetActive(true);
    }

    private float GetEffectiveRange()
    {
        switch (currentAttack)
        {
            case AttackType.Melee: return meleeRange;
            case AttackType.AOE: return aoeRange;
            case AttackType.Ranged: return rangedRange;
            case AttackType.Dash: return dashRange;
            default: return meleeRange;
        }
    }

    private bool isAttacking = false;

    private void FixedUpdate()
    {
        if (!isDetecting || currentTarget == null) return;

        Vector2 start = transform.position;
        Vector2 end = currentTarget.position;

        RaycastHit2D hit = Physics2D.Linecast(start, end, obstacleMask);

        if (hit.collider != null)
        {
            hasLineOfSight = false;
            Debug.DrawLine(start, end, Color.red);
            return;
        }

        hasLineOfSight = true;
        Debug.DrawLine(start, end, Color.green);

        Vector2 direction = (end - start).normalized;
        float distanceToTarget = Vector2.Distance(start, end);
        float effectiveRange = GetEffectiveRange();

        DrawRangeCircle(start, effectiveRange);

        if (distanceToTarget <= effectiveRange)
        {
            pathfinding.MoveTo(Vector2.zero);

            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
            {
                PickRandomAttack();
                isAttacking = true; // Start attacking
                lastAttackTime = Time.time;

                if (enemyAttack != null)
                    enemyAttack.Attack(); // Attack type depends on enemyBoss state
            }
        }
        else
        {
            if (isAttacking)
            {
                isAttacking = false;
                enemyAttack.StopAttack();
            }

            pathfinding.MoveTo(direction);
        }
    }


    public void ResetAttack()
    {
        isAttacking = false;
        enemyBoss.stopAttack();
    }

    private void PickRandomAttack()
    {
        float rng = Random.value;

        if (rng < 0.25f)
            currentAttack = AttackType.Dash;
        else if (rng < 0.5f)
            currentAttack = AttackType.AOE;
        else if (rng < 0.75f)
            currentAttack = AttackType.Ranged;
        else
            currentAttack = AttackType.Melee;

        if (enemyBoss != null)
        {
            //enemyBoss.SetCurrentAttack(currentAttack.ToString());
        }
    }

    private void DrawRangeCircle(Vector2 center, float radius)
    {
        int segments = 30;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angleStep);
            Vector3 nextPoint = center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            Debug.DrawLine(prevPoint, nextPoint, Color.red);
            prevPoint = nextPoint;
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
