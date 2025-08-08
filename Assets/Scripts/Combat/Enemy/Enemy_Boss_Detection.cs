//using UnityEngine;
//public enum BossAttack { Chase, Stomp, Powerstomp, Crush, Melee }
//public class Enemy_Boss_Detection : MonoBehaviour
//{
//    public Transform detectionArea { get; set; }
//    public bool isDetecting { get; set; }
//    private LayerMask obstacleMask;

//    public float meleeRange = 2f;
//    public float rangedRange = 6f;
//    public float aoeRange = 4f;
//    public float dashRange = 8f;

//    [HideInInspector] public Transform currentTarget;
//    [HideInInspector] public bool hasLineOfSight;

//    private Animator animator;
//    private Enemy_Pathfinding pathfinding;
//    private Enemy_Config config;
//    private Enemy_Boss enemyBoss;

//    private float attackCooldown = 2f;
//    private float lastAttackTime = -999f;

//    private void Start()
//    {
//        isDetecting = false;
//        config = GetComponent<Enemy_Config>();
//        animator = GetComponent<Animator>();
//        pathfinding = GetComponent<Enemy_Pathfinding>();
//        enemyBoss = GetComponent<Enemy_Boss>();

//        meleeRange = config.MeleeRange;
//        rangedRange = config.RangedRange;
//        detectionArea = config.DetectionArea;
//        obstacleMask = config.ObstacleMask;

//        detectionArea.gameObject.SetActive(true);
//    }

//    private BossAttack currentAttack;

//    private void PickRandomAttack()
//    {
//        float rng = Random.value;

//        if (rng < 0.25f)
//            currentAttack = BossAttack.Chase;
//        else if (rng < 0.5f)
//            currentAttack = BossAttack.Stomp;
//        else if (rng < 0.75f)
//            currentAttack = BossAttack.Powerstomp;
//        else
//            currentAttack = BossAttack.Crush;

//        if (enemyBoss != null)
//        {
//            // Inform enemyBoss of current attack if needed
//            // enemyBoss.SetCurrentAttack(currentAttack.ToString());
//        }
//    }

//    private bool isAttacking = false;
//    private float GetDesiredAttackDistance()
//    {
//        switch (currentAttack)
//        {
//            case BossAttack.Chase:
//                return dashRange;   // For chase/dash, get close but maybe further
//            case BossAttack.Stomp:
//                return meleeRange;  // Stomp = melee range (close)
//            case BossAttack.Powerstomp:
//                return aoeRange;    // Powerstomp = AOE range (mid range)
//            case BossAttack.Crush:
//                return rangedRange; // Crush = ranged attack (far)
//            case BossAttack.Melee:
//                return meleeRange;  // Melee same as stomp here
//            default:
//                return meleeRange;
//        }
//    }

//    private void FixedUpdate()
//    {
//        if (!isDetecting || currentTarget == null) return;

//        Vector2 start = transform.position;
//        Vector2 end = currentTarget.position;

//        RaycastHit2D hit = Physics2D.Linecast(start, end, obstacleMask);

//        if (hit.collider != null)
//        {
//            hasLineOfSight = false;
//            Debug.DrawLine(start, end, Color.red);
//            return;
//        }

//        hasLineOfSight = true;
//        Debug.DrawLine(start, end, Color.green);

//        Vector2 direction = (end - start).normalized;
//        float distanceToTarget = Vector2.Distance(start, end);

//        if (currentAttack == BossAttack.Chase)
//        {
//            // Chase logic: just rush toward player fast
//            pathfinding.MoveTo(direction, speedMultiplier: 2f);
//            return;  // no attack during chase itself
//        }

//        if (!isAttacking)
//        {
//            // Not currently attacking, pick a new attack and move to desired range
//            PickRandomAttack();
//        }

//        float desiredDistance = GetDesiredAttackDistance();
//        DrawRangeCircle(start, desiredDistance);

//        float margin = 0.2f;

//        if (distanceToTarget > desiredDistance + margin)
//        {
//            // Move closer
//            pathfinding.MoveTo(direction);

//            if (isAttacking)
//            {
//                isAttacking = false;
//                enemyBoss.stopAttack();
//            }
//        }
//        else if (distanceToTarget < desiredDistance - margin)
//        {
//            // Move away
//            Vector2 away = (start - end).normalized;
//            pathfinding.MoveTo(away);

//            if (isAttacking)
//            {
//                isAttacking = false;
//                enemyBoss.stopAttack();
//            }
//        }
//        else
//        {
//            // Inside desired range, stop moving and attack
//            pathfinding.MoveTo(Vector2.zero);

//            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
//            {
//                isAttacking = true;
//                lastAttackTime = Time.time;

//                if (enemyBoss != null)
//                    enemyBoss.Attack(currentAttack);
//            }
//        }
//    }

//    public void ResetAttack()
//    {
//        isAttacking = false;
//        enemyBoss.stopAttack();
//    }

    

//    private void DrawRangeCircle(Vector2 center, float radius)
//    {
//        int segments = 30;
//        float angleStep = 360f / segments;
//        Vector3 prevPoint = center + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius;

//        for (int i = 1; i <= segments; i++)
//        {
//            float rad = Mathf.Deg2Rad * (i * angleStep);
//            Vector3 nextPoint = center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
//            Debug.DrawLine(prevPoint, nextPoint, Color.red);
//            prevPoint = nextPoint;
//        }
//    }

//    private void OnTriggerStay2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {

//                currentTarget = other.transform;
//                isDetecting = true;
            
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//                currentTarget = null;
//                isDetecting = false;
            
//        }
//    }

//    private bool HasBoolParam(Animator anim, string paramName)
//    {
//        if (anim == null) return false;

//        foreach (var param in anim.parameters)
//        {
//            if (param.name == paramName && param.type == AnimatorControllerParameterType.Bool)
//                return true;
//        }
//        return false;
//    }
//}
