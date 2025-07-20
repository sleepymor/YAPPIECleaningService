using UnityEngine;
using System.Collections;

public class EnemyPatrolController : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] private float waitTimeAtPoint = 0.5f;
    [SerializeField] private bool loop = true;

    private int currentWaypointIndex = 0;
    private Enemy_Pathfinding pathfinding;
    private bool isWaiting = false;

    private void Awake()
    {
        pathfinding = GetComponent<Enemy_Pathfinding>();
    }

    private void Update()
    {
        if (Enemy_Detection.instance.hasLineOfSight != null && Enemy_Detection.instance.hasLineOfSight)
        {
            pathfinding.MoveTo(Vector2.zero);
            return;
        }

        if (isWaiting || waypoints.Length == 0) return;

        Vector2 targetPos = waypoints[currentWaypointIndex].position;
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        pathfinding.MoveTo(direction);

        if (Vector2.Distance(transform.position, targetPos) < reachDistance)
        {
            StartCoroutine(WaitAndMoveNext());
        }
    }

    private IEnumerator WaitAndMoveNext()
    {
        isWaiting = true;
        pathfinding.MoveTo(Vector2.zero);
        yield return new WaitForSeconds(waitTimeAtPoint);

        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = loop ? 0 : waypoints.Length - 1;
        }

        isWaiting = false;
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        if (loop)
        {
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}
