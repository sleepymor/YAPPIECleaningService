using UnityEngine;
using System.Collections;

public class EnemyPatrolController : MonoBehaviour
{
    private Transform[] waypoints;
    private float reachDistance;
    private float waitTimeAtPoint;
    private bool loop;

    private int currentWaypointIndex = 0;
    private Enemy_Pathfinding pathfinding;
    private bool isWaiting = false;
    private Enemy_Detection enemyDetection;
    private Enemy_Config config;

    private void Awake()
    {
        config = GetComponent<Enemy_Config>();
        pathfinding = GetComponent<Enemy_Pathfinding>();
        enemyDetection = GetComponent<Enemy_Detection>();

        waypoints = config.Waypoints;
        reachDistance = config.ReachDistance;
        waitTimeAtPoint = config.WaitTime;
        loop = config.LoopPatrol;
    }

    private void Update()
    {
        if (enemyDetection.hasLineOfSight != null && enemyDetection.hasLineOfSight)
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