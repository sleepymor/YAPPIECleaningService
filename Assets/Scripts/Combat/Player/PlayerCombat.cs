using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public Animator animator;
    private Combat combat;
    public bool isAttacking = false;

    public void Awake()
    {
        instance = this;
        combat = new Combat();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        combat.Enable();
    }

    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if(combat.Fight.Attack.triggered && !isAttacking)
        {
            isAttacking = true;
        }
    }
}
