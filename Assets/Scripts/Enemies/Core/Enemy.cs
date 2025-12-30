using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls enemy behavior using a finite state machine.
/// Handles perception (vision & sound), suspicion system,
/// and transitions between AI states.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    #region === Inspector ===
    [SerializeField] private EnemyType type;
    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private float hearingThreshold = 1f;
    [SerializeField] private int numberOfLines = 60;
    [SerializeField] private float searchTime;
    [SerializeField] private float maximumSuspicionLevel = 100f;
    [SerializeField] private float suspicionDecayOverTime = 20f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float rotationMultiplier = 30f;
    [SerializeField] private SpriteRenderer alertSign;
    [Tooltip("List of Patrol Points. EXCLUDING spawn point")]
    [SerializeField] private List<Transform> patrolPositionsList;
    [SerializeField] private float timeTOCheckNewSound = 0.4f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private EnemyFOVVisualizer enemyView;
    #endregion


    #region === Runtime Data ===
    private Rigidbody2D rb;
    protected EnemyState state;
    private LineRenderer zoneRenderer;
    private float detectTimer = 0f;
    private float moveSpeed;
    private float attackRadius;
    private float maxHealth;
    private float attackCooldDown;
    private float attackDamage;
    private List<Vector3> checkedPositions;
    private EnemyBehaveType behaveType;
    #endregion

    #region === Properties ===
    public float MoveSpeed => moveSpeed;
    public List<Transform> PatrolPositionsList => patrolPositionsList;
    public Vector3 StartPosition { get; private set; }
    public float StartRotation { get; private set; }
    public Transform CurrentSeenTargetPosition { get; private set; }
    public Noise CurrentNoise { get; private set; }
    public float SearchTime => searchTime;
    public float AttackRadius => attackRadius;
    public float AttackCooldown => attackCooldDown;
    public float AttackDamage => attackDamage;
    public float CurrentNoiseStrength { get; private set; }
    public float LastNoiseTime { get; private set; }
    public float Suspicion { get; private set; }
    public bool CanSeeTarget { get; private set; }
    public LayerMask PlayerLayer => playerLayer;
    public Rigidbody2D Rb => rb;
    #endregion

    #region === Constant Variables ===
    public const float HighSuspicion = 60f;
    public const float MiddleSuspicion = 40f;
    public const float LowSuspicion = 20f;
    #endregion


    #region === Unity Callbacks ===
    private void Awake()
    {
        attackRadius = type.attackRange;
        moveSpeed = type.moveSpeed;
        maxHealth = type.maxHealthPoint;
        attackCooldDown = type.attackCoolDown;
        attackDamage = type.attackDamage;
        behaveType = type.behaveType;
        enemyView.ViewRadius = type.viewRadius;
        checkedPositions = new List<Vector3>();
        CanSeeTarget = false;
        zoneRenderer = GetComponent<LineRenderer>();
        zoneRenderer.startColor = Color.yellow;
        zoneRenderer.positionCount = numberOfLines + 1;
        StartPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        StartRotation = rb.rotation;

        if (behaveType == EnemyBehaveType.Guard)
            state = new EnemyIdle(this);
        else
            state = new EnemyPatrol(this);

        state.Enter();
    }

    private void FixedUpdate()
    {
        HandleSuspicionUi();
        HandleNoiseDetection();

        UpdateSuspicion();

        state?.Execute();

        CreateZone();

        if (CanSeeTarget && CurrentSeenTargetPosition != null)
        {
            RotateToPoint(CurrentSeenTargetPosition.position);
        }
    }
    #endregion

    #region === State Machine ===
    public void ChangeState(EnemyState state)
    {
        this.state?.Exit();
        this.state = state;
        this.state?.Enter();
    }

    public EnemyState EvaluateThreat()
    {
        if (CanSeeTarget && Suspicion > LowSuspicion)
            return new EnemyChase(this);
        if (Suspicion > HighSuspicion)
            return new EnemyInvestigate(this);
        if (Suspicion > LowSuspicion)
            return new EnemySuspision(this);

        return null;
    }

    public EnemyState ReturnDefaultState()
    {
        if (behaveType == EnemyBehaveType.Guard)
            return new EnemyIdle(this);
        else
            return new EnemyPatrol(this);
    }
    #endregion

    #region === Perception ===
    public void SetFovData(bool canSee, Transform seenTarget)
    {
        CanSeeTarget = canSee;

        if (CanSeeTarget && seenTarget != null)
        {
            CurrentSeenTargetPosition = seenTarget;
        }
    }

    public Noise DetectNoise()
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius);
        Noise bestNoise = null;
        float bestStrenth = 0f;


        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                Noise noise = hit.GetComponent<Noise>();
                if (noise == null) continue;

                float distance = Vector2.Distance(transform.position, noise.transform.position);


                if (distance <= noise.NoiseRadius && noise.NoiseStrength > hearingThreshold && noise.NoiseStrength > bestStrenth)
                {
                    bestStrenth = noise.NoiseStrength;
                    zoneRenderer.startColor = Color.red;
                    bestNoise = noise;
                }
                else
                    zoneRenderer.startColor = Color.yellow;
            }
        }

        return bestNoise;
    }

    private void HandleNoiseDetection()
    {
        if (!CanSeeTarget && state is not EnemyChase)
        {
            if (detectTimer >= timeTOCheckNewSound)
            {
                detectTimer = 0f;
                CurrentNoise = DetectNoise();

                if (CurrentNoise != null)
                    SetCurrentNoise(CurrentNoise);
            }
            else
            {
                detectTimer += Time.fixedDeltaTime;
            }
        }
    }

    public void SetCurrentNoise(Noise noise)
    {
        if (!CanSeeTarget)
        {
            if (checkedPositions.Contains(noise.transform.position)) return;
            checkedPositions.Add(noise.transform.position);
            CurrentNoiseStrength = noise.NoiseStrength;
        }

        LastNoiseTime = Time.time;
    }

    public void CLearCurrentNoise()
    {
        CurrentNoise = null;
    }

    private void CreateZone()
    {
        float angleStep = 360f / numberOfLines;

        for (int i = 0; i <= numberOfLines; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * checkRadius;
            pos += transform.position;
            zoneRenderer.SetPosition(i, pos);
        }
    }
    #endregion

    #region === Suspicion System ===
    private void UpdateSuspicion()
    {
        if (CanSeeTarget)
            Suspicion += MiddleSuspicion * Time.fixedDeltaTime;
        else if (CurrentNoise != null)
        {
            Suspicion += CurrentNoise.NoiseStrength * Time.fixedDeltaTime * 10;
        }
        else
            Suspicion -= suspicionDecayOverTime * Time.fixedDeltaTime;

        Suspicion = Mathf.Clamp(Suspicion, 0, maximumSuspicionLevel);
    }

    private void HandleSuspicionUi()
    {
        if (Suspicion > 0)
        {
            if (!alertSign.gameObject.activeInHierarchy)
                alertSign.gameObject.SetActive(true);

            if (Suspicion < LowSuspicion)
                alertSign.color = Color.green;
            else if (Suspicion > LowSuspicion && Suspicion < HighSuspicion)
                alertSign.color = Color.yellow;
            else if (Suspicion > HighSuspicion)
                alertSign.color = Color.red;

        }
        else
        {
            alertSign.gameObject.SetActive(false);
            Suspicion = 0;
        }
    }
    #endregion

    #region === Movement & Rotation ===
    public void InvestigateAround()
    {
        if (CanSeeTarget) return;

        float angle = rb.rotation + rotateSpeed * Time.fixedDeltaTime * rotationMultiplier;
        rb.MoveRotation(angle);
    }

    public void RotateToPoint(Vector2 targetPoint)
    {
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;


        float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotateSpeed * Time.fixedDeltaTime * rotationMultiplier);
        rb.MoveRotation(angle);
    }

    public void ReturnToDefault()
    {
        if (CanSeeTarget) return;

        float angle = Mathf.MoveTowardsAngle(rb.rotation, StartRotation, rotateSpeed * Time.fixedDeltaTime * rotationMultiplier);

        rb.MoveRotation(angle);
    }

    public void ClearCheckedPositions()
    {
        checkedPositions.Clear();
    }
    #endregion
}
