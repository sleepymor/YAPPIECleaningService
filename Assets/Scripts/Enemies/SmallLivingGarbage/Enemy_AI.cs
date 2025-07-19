using UnityEngine;
using System.Collections;

public class Enemy_AI : MonoBehaviour
{
    public static Enemy_AI instance;

    public enum State
    {
        Roaming,
        Chasing,
    }

    public State state;
    private Enemy_Pathfinding pathfinding;
    private bool hasLineOfSight = false;

    [SerializeField] private Transform player;
    [SerializeField] private float chaseUpdateRate = 0.5f;

    private void Awake()
    {
        pathfinding = GetComponent<Enemy_Pathfinding>();
    }

    private void Start()
    {
        SetState(State.Roaming);
    }

    private void Update()
    {
        if (hasLineOfSight)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 2f * Time.deltaTime);
        }
    }


    [SerializeField] private LayerMask obstacleMask;

    private void FixedUpdate()
    {
        Vector2 start = transform.position;
        Vector2 end = player.position;

        RaycastHit2D hit = Physics2D.Linecast(start, end, obstacleMask);

        if (hit.collider == null)
        {
            // Nothing blocked the ray — assume player in sight
            hasLineOfSight = true;
            Debug.DrawLine(start, end, Color.green);
        }
        else
        {
            // Something blocked the view
            hasLineOfSight = false;
            Debug.DrawLine(start, end, Color.red);
        }
    }




    public void SetState(State newState)
    {
        state = newState;

        StopAllCoroutines();

        switch (state)
        {
            case State.Roaming:
                StartCoroutine(RoamingRoutine());
                break;
            case State.Chasing:
                StartCoroutine(ChasingRoutine());
                break;
        }
    }

    private IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            pathfinding.MoveTo(roamPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator ChasingRoutine()
    {
        while (state == State.Chasing)
        {
            //pathfinding.MoveTo(player.position);
            yield return new WaitForSeconds(chaseUpdateRate);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        return (Vector2)transform.position + randomDir * Random.Range(1f, 3f);
    }
}
