using UnityEngine;

// We can reuse the same enum from before
public enum EnemyState
{
    Moving,
    Idle,
    Stopped 
}

public class Enemy_AI : MonoBehaviour
{
    [Header("State Machine")]
    public EnemyState currentState;

    [Header("Movement Settings")]
    public float speed = 2f;

    public float moveTime = 2.0f; 

    [Header("Idle Settings")]

    public float idleTime = 3.0f; 

    private float stateTimer;
 
    private Vector2 moveDirection;

    void Start()
    {

        currentState = EnemyState.Idle;
        stateTimer = idleTime;
    }

    void Update()
    {
  
        switch (currentState)
        {
            case EnemyState.Moving:
    
                transform.Translate(moveDirection * speed * Time.deltaTime);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {

                    currentState = EnemyState.Idle;
                    stateTimer = idleTime;
                }
                break;

            case EnemyState.Idle:

                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
   
                    ChooseNewDirection();
                    currentState = EnemyState.Moving;
                    stateTimer = moveTime;
                }
                break;

            case EnemyState.Stopped:

                break;
        }
    }

    // This function picks a random cardinal direction
    private void ChooseNewDirection()
    {
        int randomDirection = Random.Range(0, 4);

        switch (randomDirection)
        {
            case 0:
                moveDirection = Vector2.up;    
                break;
            case 1:
                moveDirection = Vector2.down; 
                break;
            case 2:
                moveDirection = Vector2.left; 
                break;
            case 3:
                moveDirection = Vector2.right;
                break;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy hit by Player! Stopping movement.");
            currentState = EnemyState.Stopped;
        }
    }
}
