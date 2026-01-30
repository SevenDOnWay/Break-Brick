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


    public event Action<BallScript> OnBallFinished;
    private TrailRenderer trail;

    [Header("Stat")]
    float speed;
    float critChance;
    float critMultiplier;
    float fireChance;
    float lightningChance;
    float explosiveChance;

    private Dictionary<UpgradeType, FieldInfo> statFieldMap;
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
        currentProcesses = upgradeManager.GetAllProcess();
    }

    private void Awake() {
        collider2D = GetComponent<Collider2D>();
        trail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        NewLauch();

        //BuildReflectionMap();

        speed = statManager.GetStat(UpgradeType.Speed);

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


    public void LaunchBall( Vector2 dir ) => rb.AddForce(dir * speed, ForceMode2D.Impulse);

    void NewLauch() {
        collider2D.enabled = true;
        bounceTime = 0;
        endLineTriggerCount = 0;
        EnableTrail(true);
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

            brick.NotifyHit(DamageSource.Ball);
            bounceTime = 0;
        }
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
        currentProcesses.Add(process);
    }

}
