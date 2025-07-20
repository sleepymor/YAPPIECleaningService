using UnityEngine;

public class Enemy_Detection : MonoBehaviour
{
    public static Enemy_Detection instance;

    [SerializeField] public Transform detectionArea;
    [SerializeField] public bool isDetecting = false;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] public float attackRange = 1.5f;

    [HideInInspector] public Transform currentTarget;
    [HideInInspector] public bool hasLineOfSight;

    private Enemy_Pathfinding pathfinding;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pathfinding = GetComponent<Enemy_Pathfinding>();
        detectionArea.gameObject.SetActive(true);
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

                int segments = 30;
                float angleStep = 360f / segments;
                Vector3 prevPoint = start + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * attackRange;
                for (int i = 1; i <= segments; i++)
                {
                    float rad = Mathf.Deg2Rad * (i * angleStep);
                    Vector3 nextPoint = start + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * attackRange;
                    Debug.DrawLine(prevPoint, nextPoint, Color.red);
                    prevPoint = nextPoint;
                }

                if (distanceToTarget <= attackRange)
                {
                    pathfinding.MoveTo(Vector2.zero);
                    Enemy_Attack.instance.Attack();
                }
                else
                {
                    Enemy_Attack.instance.StopAttack();
                    pathfinding.MoveTo(direction);
                }
            }

            else
            {
                hasLineOfSight = false;
                Debug.DrawLine(start, end, Color.red);
            }
        }
        else
        {
            hasLineOfSight = false;

        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.name == "Hitbox")
        {
            Transform playerRoot = other.transform.root;
            if (playerRoot.GetComponent<PlayerController>())
            {
                var player = other;
                currentTarget = player.transform;
                isDetecting = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Hitbox")
        {
            Transform playerRoot = other.transform.root;
            if (playerRoot.GetComponent<PlayerController>())
            {
                var player = other;
                currentTarget = null;
                isDetecting = false;
            }
        }
    }

}
