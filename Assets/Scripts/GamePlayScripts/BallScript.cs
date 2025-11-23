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

    private Dictionary<UpgradeType, FieldInfo> statFieldMap;


    public void Init(BallManager ballManager, StatManager statManager,float squareSize) {
        this.ballManager = ballManager;
        this.squareSize = squareSize;
        this.statManager = statManager;
    }

    private void Awake() {
        collider2D = GetComponent<Collider2D>();
        trail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        TrailInitialize();
        NewLauch();

        //BuildReflectionMap();

        speed = statManager.GetStat(UpgradeType.Speed);

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

    //TODO: serializefeild trail settings
    void TrailInitialize() {
        trail.time = 0.2f;
        trail.startWidth = 0.15f;
        trail.endWidth = 0.05f;
        trail.minVertexDistance = 0.05f;


        //TODO: serializefeild gradient settings
        Gradient gradient = new();
        gradient.SetKeys(
            new GradientColorKey[] {
            new(Color.white, 0.0f),
            new(Color.white, 1.0f)
            },
            new GradientAlphaKey[] {
            new(1.0f, 0.0f),
            new(0.0f, 1.0f)
            }
        );
        trail.colorGradient = gradient;
    }

    public void LaunchBall(Vector2 dir) => rb.AddForce(dir * speed, ForceMode2D.Impulse);

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
            }
        }

        // ===== BrickScript Collision Logic =====
        if ( collision.gameObject.TryGetComponent<BrickScript>(out BrickScript brick) ) {



            brick.TakeDamage(1);
            bounceTime = 0;
        }
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if ( collision.gameObject.CompareTag("EndLine") ) {
            endLineTriggerCount++;
            if ( endLineTriggerCount <= maxEndLineTriggers ) return;

            Vector2 newPos = new Vector2(transform.position.x,squareSize * -6);
            ballManager.ResetBallPos(newPos);

            FinishBall();
        }
    }

    async void FinishBall() {
        EnableTrail(false);
        await ResetVelocityCoroutine();
        NewLauch();
    }

    async Task ResetVelocityCoroutine() {
        collider2D.enabled = false;
        rb.linearVelocity = Vector2.zero;
        await transform.DOMove(ballManager.ballPos, duration).SetEase(Ease.InOutSine)
            .AsyncWaitForCompletion();

        OnBallFinished?.Invoke(this);
    }

    public void SetProperties( Dictionary<string, float> properties ) {
        // need per-ball state later, can cache properties here
    }

}
