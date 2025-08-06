using UnityEngine;

public enum AttackStatus { None, Stun }
public enum EnemyEnvironment { Beach, FloatingCity, DeadCoralReefs}

public class Enemy_Config : MonoBehaviour
{
    [SerializeField] private string killMissionName = "";
    [Header("Basic Settings")]
    [Tooltip("Basic settings of the enemy.")]
    [SerializeField] private int maximumHealth = 3;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float knockBackRange = 2f;
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float rangedRange = 4f;
    [SerializeField] private bool canHarpooned = true;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private EnemyEnvironment environment = EnemyEnvironment.Beach;

    [SerializeField] private int meleeDamage = 1;
    [SerializeField] private AttackStatus meleeAttackStatus = AttackStatus.None;
    [SerializeField] private float meleeAttackStatusChance = 0f;
    [SerializeField] private float meleeAttackStatusDuration = 0f;

    [SerializeField] private int rangedDamage = 1;
    [SerializeField] private AttackStatus rangedAttackStatus = AttackStatus.None;
    [SerializeField] private float rangedAttackStatusChance = 0f;
    [SerializeField] private float rangedAttackStatusDuration = 0f;
    [SerializeField] private int aoeDamage = 1;
    [SerializeField] private AttackStatus aoeAttackStatus = AttackStatus.None; 
    [SerializeField] private float aoeAttackStatusChance = 0f;
    [SerializeField] private float aoeAttackStatusDuration = 0f;
    [SerializeField] private AttackMode skill = AttackMode.Melee;
    [SerializeField] private AttackMode basicAttack = AttackMode.Melee;


    [Tooltip("Sprite Orientation")]
    [SerializeField] private bool spriteFacingRight = true;

    [Header("Advanced Settings")]

    [Tooltip("Projectile Settings (Ranged)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int projectileCount = 5;
    [SerializeField] private float spreadAngle = 30f;
    [SerializeField] private float rangedProjectileSpeed = 10f;
    [SerializeField] private float maxTravelDistance = 10f;

    [Tooltip("Hiding mode for enemy")]
    [SerializeField] private bool hasHidingMode = false;
    [SerializeField] private bool hidingMode = false;
    [SerializeField] private bool useTrigger = false;
    [SerializeField] private float triggerRange = 1.5f;

    [Header("Death Animation")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int numberOfFlashes = 5;

    [Header("Component Settings")]
    [Tooltip("Area for enemy to detect player")]
    [SerializeField] private Transform detectionArea;
    [Tooltip("Vision blocker for player to hide from enemy")]
    [SerializeField] private LayerMask obstacleMask;
    [Tooltip("Waypoints for enemy patrols")]
    [SerializeField] private Transform[] waypoints;
    [Tooltip("How far the enemy would stop at every way point for waiting")]
    [SerializeField] private float reachDistance = 0.1f;
    [Tooltip("Waiting time in waypoint before patrol again")]
    [SerializeField] private float waitTimeAtPoint = 0.5f;
    [SerializeField] private bool loopPatrol = true;

    [Header("AOE Settings")]
    [SerializeField] private GameObject aoePrefab;
    [SerializeField] private float aoeCooldown = 10f;
    [SerializeField] private bool immediatelyTriggered = false;

    [SerializeField] private float aoeFireRate = 0.1f;
    [SerializeField] private float aoeDuration = 2f;
    [SerializeField] private float aoeProjectileSpeed = 2f;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float aoeLifetime = 1f;




    // Public Getters
    public string KillMissionName => killMissionName;

    public int MaximumHealth => maximumHealth;
    public float MoveSpeed => moveSpeed;
    public int MeleeDamage => meleeDamage;
    public int RangedDamage => rangedDamage;
    public int AOEDamage => aoeDamage;
    public float KnockbackRange => knockBackRange;
    public bool SpriteFacingRight => spriteFacingRight;
    public float MeleeRange => meleeRange;
    public float RangedRange => rangedRange;
    public bool CanHarpooned => canHarpooned;
    public bool IsBoss => isBoss;
    public EnemyEnvironment Environment => environment;

    public AttackStatus MeleeAttackStatus => meleeAttackStatus;
    public float MeleeAttackStatusChance => meleeAttackStatusChance;
    public float MeleeAttackStatusDuration => meleeAttackStatusDuration;

    public AttackStatus RangedAttackStatus => rangedAttackStatus;
    public float RangedAttackStatusChance => rangedAttackStatusChance;
    public float RangedAttackStatusDuration => rangedAttackStatusDuration;

    public AttackStatus AOEAttackStatus => aoeAttackStatus;
    public float AOEAttackStatusChance => aoeAttackStatusChance;
    public float AOEAttackStatusDuration => aoeAttackStatusDuration;


    public AttackMode Skill => skill;
    public AttackMode BasicAttack => basicAttack;

    public GameObject ProjectilePrefab => projectilePrefab;
    public int ProjectileCount => projectileCount;
    public float SpreadAngle => spreadAngle;
    public float RangedProjectileSpeed => rangedProjectileSpeed;
    public float MaxProjTravelDistance => maxTravelDistance;

    public bool HasHidingMode => hasHidingMode;
    public bool HidingMode => hidingMode;
    public bool UseTrigger => useTrigger;
    public float TriggerRange => triggerRange;

    public float FlashDuration => flashDuration;
    public int NumberOfFlashes => numberOfFlashes;

    public Transform DetectionArea => detectionArea;
    public LayerMask ObstacleMask => obstacleMask;

    public Transform[] Waypoints => waypoints;
    public float ReachDistance => reachDistance;
    public float WaitTime => waitTimeAtPoint;
    public bool LoopPatrol => loopPatrol;

    public GameObject AOEPrefab => aoePrefab;
    public float AOECooldown => aoeCooldown;
    public bool ImmediatelyTriggered => immediatelyTriggered;

    public float AOEFireRate => aoeFireRate;
    public float AOEDuration => aoeDuration;
    public float AOEProjectileSpeed => aoeProjectileSpeed;
    public float AOEDamageInterval => damageInterval;
    public float MaxAOELifetime => aoeLifetime;

}
