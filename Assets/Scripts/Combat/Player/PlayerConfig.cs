using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    public static PlayerConfig c;

    void Awake()
    {
        c = this;
    }

    [Header("Basic Settings")]
    [Tooltip("Basic settings of the player.")]
    [SerializeField] private int maximumHealth = 10;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int meleeDamage = 1;
    [SerializeField] private int harpoonDamage = 1;
    [SerializeField] private float knockBackRange = 2f;

    [Header("Advanced Settings")]
    [Tooltip("Advanced Settings for the player")]
    [SerializeField] private float attackMoveSpeed = 3f;
    [SerializeField] private float attackMoveDuration = 0.2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Harpoon")]
    [SerializeField] private float harpoonRange = 4f;
    [SerializeField] private float harpoonHoldTime = 0.5f;
    [SerializeField] private float harpoonCooldown = 1f;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float invincibilityFlashDuration = 0.1f;


    [SerializeField] private Transform meleeDmgArea;
    [SerializeField] private Transform harpoonDmgArea;
    [SerializeField] private GameObject harpoonAimPrefab;

    // Public Getters
    public int MaximumHealth => maximumHealth;
    public float MoveSpeed => moveSpeed;
    public int MeleeDamage => meleeDamage;
    public int HarpoonDamage => harpoonDamage;

    public float MeleeAttackMoveSpeed => attackMoveSpeed;
    public float MeleeAttackMoveDuration => attackMoveDuration;

    public float HarpoonRange => harpoonRange;
    public float HarpoonHoldTime => harpoonHoldTime;
    public float HarpoonCooldown => harpoonCooldown;

    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;

    public float InvincibilityDuration => invincibilityDuration;
    public float InvincibilityFlashDuration => invincibilityFlashDuration;

    public Transform MeleeDmgArea => meleeDmgArea;
    public Transform HarpoonDmgArea => harpoonDmgArea;
    public GameObject HarpoonAimPrefab => harpoonAimPrefab;
}
