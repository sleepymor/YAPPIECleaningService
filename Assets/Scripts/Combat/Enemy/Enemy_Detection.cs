using UnityEngine;

public class Enemy_Detection : MonoBehaviour
{
    public Transform detectionArea { get; set; }
    public bool isDetecting { get; set; }
    private LayerMask obstacleMask;

    public float meleeRange { get; set; }
    public float rangedRange { get; set; }
    private bool useTrigger;
    private float triggerRange;

    [HideInInspector] public Transform currentTarget;
    [HideInInspector] public bool hasLineOfSight;

    private Animator animator;
    private Enemy_Pathfinding pathfinding;
    private Enemy_Attack enemyAttack;
    private Enemy_Config config;

    private void Start()
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
        obstacleMask = config.ObstacleMask;

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

            RaycastHit2D hit = Physics2D.Linecast(start, end, obstacleMask);

            if (hit.collider == null)
            {
                hasLineOfSight = true;
                Debug.DrawLine(start, end, Color.green);

                Vector2 direction = (end - start).normalized;
                float distanceToTarget = Vector2.Distance(start, end);
                float effectiveRange = GetEffectiveRange(); // Uses smart logic

                DrawRangeCircle(start, effectiveRange);

                if (distanceToTarget <= effectiveRange)
                {

                    if (HasBoolParam(animator, "isHiding") && useTrigger && distanceToTarget <= triggerRange)
                    { 
                        animator.SetBool("isHiding", false);
                    } else
                    {
                        Debug.Log("False");
                    }

                    pathfinding.MoveTo(Vector2.zero);
                    enemyAttack.Attack();
                }
                else
                {
                    enemyAttack.StopAttack();
                    pathfinding.MoveTo(direction);
                }
            }
            else
            {
                hasLineOfSight = false;
                Debug.DrawLine(start, end, Color.red);
            }
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
        if (other.transform.name == "Hitbox" && other.transform.root.GetComponent<PlayerController>() != null)
        {
            Transform playerRoot = other.transform.root;
            if (playerRoot.GetComponent<PlayerController>())
            {
                currentTarget = other.transform;
                isDetecting = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.name == "Hitbox" && other.transform.root.GetComponent<PlayerController>() != null)
        {
            Transform playerRoot = other.transform.root;
            if (playerRoot.GetComponent<PlayerController>())
            {
                currentTarget = null;
                isDetecting = false;
            }
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
