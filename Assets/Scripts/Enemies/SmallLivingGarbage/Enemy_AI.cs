using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_AI : MonoBehaviour
{
    private enum State
    {
        Roaming
    }

    private State state;
    private Enemy_Pathfinding pathfinding;

    private void Awake()
    {
        pathfinding = GetComponent<Enemy_Pathfinding>();
        state = State.Roaming;
    }

    private void Start()
    {
        StartCoroutine(RoamingRoutine());
    }

    private IEnumerator RoamingRoutine()
    {
        while(state == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            pathfinding.MoveTo(roamPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
} 
