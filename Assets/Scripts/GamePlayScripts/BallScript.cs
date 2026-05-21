using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(TrailRenderer))]
public class BallScript : MonoBehaviour {

    BallManager ballManager;
    StatManager statManager;
    UpgradeManager upgradeManager;

    [HideInInspector] public Rigidbody2D rb;
    Collider2D collider2D;
    int bounceTime;
    int endLineTriggerCount = 0;
    const int maxEndLineTriggers = 2;
    float duration = 0.5f; // duration for moving back to start position



    float squareSize;

    BrickScript lastBrickScript;
    int lastCollisionFrame = -1;
    int specialEffectFrame = -1;
    readonly HashSet<SpecialEffectTriggerKey> triggeredSpecialEffectsThisFrame = new();


    public event Action<BallScript> OnBallFinished;
    private TrailRenderer trail;
    private SpriteRenderer spriteRenderer;
    private Color defaultTrailStartColor;
    private Color defaultTrailEndColor;
    private Color defaultSpriteColor;

    [Header("Stat")]
    float speed;
    float baseDamage;

    [Header("Special Ball")]
    [SerializeField] SpecialBallConfig specialBallConfig;
    int remainingPierces;
    Vector2 lastVelocity;

    private List<Process> currentProcesses = new List<Process>();

    public void Init( BallManager ballManager,
        StatManager statManager,
        UpgradeManager upgradeManager,
        float squareSize ) {
        this.ballManager = ballManager;
        this.statManager = statManager;
        this.upgradeManager = upgradeManager;
        this.squareSize = squareSize;

        InitCurrentUpgrade(upgradeManager);
    }

    private void InitCurrentUpgrade( UpgradeManager upgradeManager ) {
        // Create a copy to avoid double-adding processes when OnProcessAdded event fires
        currentProcesses.Clear();

        if ( upgradeManager == null ) {
            return;
        }

        foreach ( var process in upgradeManager.GetAllProcess() ) {
            AddProcess(process);
        }
    }

    private void Awake() {
        collider2D = GetComponent<Collider2D>();
        trail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if ( trail != null ) {
            defaultTrailStartColor = trail.startColor;
            defaultTrailEndColor = trail.endColor;
        }

        if ( spriteRenderer != null ) {
            defaultSpriteColor = spriteRenderer.color;
        }
    }

    void Start() {
        NewLauch();

        //BuildReflectionMap();

        speed = GetConfiguredSpeed();
        baseDamage = statManager.GetStat(UpgradeType.BaseDamage);

        upgradeManager.OnProcessAdded += AddProcess;
    }

    //TOOD: use reflection to map stat fields

    //public void BuildReflectionMap() {
    //    statFieldMap = new();

    //    var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    //    foreach ( var field in fields ) {
    //        var attr = field.GetCustomAttribute<StatFieldAttribute>();
    //        if ( attr != null ) {
    //            statFieldMap[attr.Type] = field;
    //        }
    //    }
    //}

    void EnableTrail( bool enable ) {
        trail.emitting = enable;
    }


    void FixedUpdate() {
        if ( rb != null && rb.linearVelocity.sqrMagnitude > 0.001f ) {
            lastVelocity = rb.linearVelocity;
        }
    }

    public void LaunchBall( Vector2 dir ) {
        speed = GetConfiguredSpeed();
        rb.AddForce(dir * speed, ForceMode2D.Impulse);
    }

    void NewLauch() {
        collider2D.enabled = true;
        bounceTime = 0;
        endLineTriggerCount = 0;
        lastBrickScript = null;
        lastCollisionFrame = -1;
        specialEffectFrame = -1;
        triggeredSpecialEffectsThisFrame.Clear();
        remainingPierces = specialBallConfig != null ? specialBallConfig.PierceLimit : 0;
        EnableTrail(true);
        ApplyConfiguredVisuals();
        ResetProcesses();
    }

    private void OnCollisionEnter2D( Collision2D collision ) {
        // ===== Wall Collision Logic =====
        if ( collision.gameObject.CompareTag("Wall") ) {
            bounceTime++;
            if ( bounceTime > 5 ) {
                FinishBall();
                return;
            }
        }

        if ( collision.gameObject.CompareTag("Bottom_Wall") ) {

            Vector2 newPos = new Vector2(transform.position.x,squareSize * -11 / 2);
            ballManager.ResetBallPos(newPos);

            FinishBall();
            return;
        }

        // ===== BrickScript Collision Logic =====
        if ( collision.gameObject.TryGetComponent<BrickScript>(out BrickScript brick) ) {

            // Prevent processing the same collision multiple times in the same frame
            if ( brick == lastBrickScript && Time.frameCount == lastCollisionFrame ) {
                return;
            }

            lastBrickScript = brick;
            lastCollisionFrame = Time.frameCount;

            baseDamage = statManager.GetStat(UpgradeType.BaseDamage);
            int directDamage = GetConfiguredDirectDamage((int)baseDamage);
            int bonusDamage = ApplyProcess(brick, directDamage);

            Vector2 hitNormal = collision.contacts.Length > 0 ? collision.contacts[0].normal : Vector2.zero;
            var hitContext = new BallHitContext(this, brick, collision, statManager, directDamage, hitNormal, squareSize);
            specialBallConfig?.ApplyHitEffects(hitContext);

            brick.NotifyHit(DamageSource.Ball, directDamage + bonusDamage, hitNormal);
            Debug.Log("Ball hit brick at " + brick.GridPosition + " with direct damage " + directDamage + " and bonus damage " + bonusDamage);
            bounceTime = 0;

            if ( TryPierceBrick(collision) ) {
                return;
            }
        }
    }

    public int ApplyProcess( BrickScript brick, int baseDamage ) {
        int totalBonus = 0;
        foreach ( var process in currentProcesses ) {
            totalBonus += process.OnHit(statManager, brick, baseDamage);
        }
        return totalBonus;
    }


    //private void OnTriggerEnter2D( Collider2D collision ) {
    //    if ( collision.gameObject.CompareTag("Bottom_Wall") ) {

    //        Vector2 newPos = new Vector2(transform.position.x,squareSize * -6);
    //        ballManager.ResetBallPos(newPos);

    //        FinishBall();
    //    }
    //}

    void FinishBall() {
        EnableTrail(false);
        collider2D.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // Use DOTween's OnComplete instead of async/await
        transform.DOMove(ballManager.ballPos, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {
                NewLauch(); // Prepare for next round
                OnBallFinished?.Invoke(this); // Fire event
            });
    }

    void AddProcess( Process process ) {
        if ( process == null || currentProcesses.Contains(process) ) {
            return;
        }

        currentProcesses.Add(process);
    }

    public void SetSpecialBallConfig( SpecialBallConfig config ) {
        specialBallConfig = config;
        remainingPierces = specialBallConfig != null ? specialBallConfig.PierceLimit : 0;
        ApplyConfiguredVisuals();
    }

    public BallType GetBallType() {
        return specialBallConfig != null ? specialBallConfig.BallType : BallType.Normal;
    }

    public void ApplySpecialBallColor( Color color ) {
        if ( spriteRenderer != null ) {
            spriteRenderer.color = color;
        }

        if ( trail != null ) {
            trail.startColor = color;
            trail.endColor = new Color(color.r, color.g, color.b, Mathf.Min(color.a, defaultTrailEndColor.a));
        }
    }

    internal bool TryMarkSpecialEffect( BallHitEffect effect, BrickScript brick ) {
        if ( effect == null || brick == null ) {
            return false;
        }

        if ( specialEffectFrame != Time.frameCount ) {
            specialEffectFrame = Time.frameCount;
            triggeredSpecialEffectsThisFrame.Clear();
        }

        SpecialEffectTriggerKey key = new(effect.GetType(), brick.GetInstanceID());
        return triggeredSpecialEffectsThisFrame.Add(key);
    }

    readonly struct SpecialEffectTriggerKey : IEquatable<SpecialEffectTriggerKey> {
        readonly Type effectType;
        readonly int brickInstanceId;

        public SpecialEffectTriggerKey( Type effectType, int brickInstanceId ) {
            this.effectType = effectType;
            this.brickInstanceId = brickInstanceId;
        }

        public bool Equals( SpecialEffectTriggerKey other ) {
            return effectType == other.effectType && brickInstanceId == other.brickInstanceId;
        }

        public override bool Equals( object obj ) {
            return obj is SpecialEffectTriggerKey other && Equals(other);
        }

        public override int GetHashCode() {
            return unchecked(((effectType != null ? effectType.GetHashCode() : 0) * 397) ^ brickInstanceId);
        }
    }

    float GetConfiguredSpeed() {
        float statSpeed = statManager != null ? statManager.GetStat(UpgradeType.Speed) : speed;
        float multiplier = specialBallConfig != null ? specialBallConfig.SpeedMultiplier : 1f;
        return statSpeed * multiplier;
    }

    int GetConfiguredDirectDamage( int damage ) {
        return specialBallConfig != null ? specialBallConfig.GetDirectDamage(damage) : damage;
    }

    void ApplyConfiguredVisuals() {
        if ( specialBallConfig != null && specialBallConfig.BallType != BallType.Normal ) {
            specialBallConfig.ApplyVisuals(this);
            return;
        }

        if ( spriteRenderer != null ) {
            spriteRenderer.color = defaultSpriteColor;
        }

        if ( trail != null ) {
            trail.startColor = defaultTrailStartColor;
            trail.endColor = defaultTrailEndColor;
        }
    }

    bool TryPierceBrick( Collision2D collision ) {
        if ( specialBallConfig == null || specialBallConfig.BallType != BallType.Piercing || remainingPierces <= 0 ) {
            return false;
        }

        remainingPierces--;

        if ( collision.collider != null ) {
            Physics2D.IgnoreCollision(collider2D, collision.collider, true);
            StartCoroutine(RestoreCollision(collision.collider));
        }

        if ( lastVelocity.sqrMagnitude > 0.001f ) {
            rb.linearVelocity = lastVelocity;
        }

        return true;
    }

    IEnumerator RestoreCollision( Collider2D ignoredCollider ) {
        yield return new WaitForSeconds(0.2f);

        if ( collider2D != null && ignoredCollider != null ) {
            Physics2D.IgnoreCollision(collider2D, ignoredCollider, false);
        }
    }

    /// <summary>
    /// Resets per-turn counters on all processes (e.g. Sniper, Rally).
    /// </summary>
    void ResetProcesses() {
        foreach ( var process in currentProcesses ) {
            process.Reset();
        }
    }
}
