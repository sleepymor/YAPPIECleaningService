using UnityEngine;
using System.Collections;

public class Enemy_AI : MonoBehaviour
{
    public static Enemy_AI instance;

    private Enemy_Pathfinding pathfinding;
    private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private float chaseUpdateRate = 0.5f;
    [SerializeField] private float attackSpeed = 1f;

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
        pathfinding = GetComponent<Enemy_Pathfinding>();
    }

    private void Start()
    {
    }

    private void Update()
    {
       
    }
}
