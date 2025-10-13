using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(TrailRenderer))]
public class BallScript : MonoBehaviour
{

    [Inject] BallManager ballManager;


    [HideInInspector] public Rigidbody2D rb;
    Collider2D collider2D;
    int bounceTime;
    int endLineTriggerCount = 0;
    const int maxEndLineTriggers = 2;
    float duration = 0.5f; // duration for moving back to start position

    public event Action<BallScript> OnBallFinished;

    private TrailRenderer trail;

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        trail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        TrailInitialize();
        NewLauch();
    }

    void EnableTrail(bool enable)
    {
        trail.emitting = enable;
    }

    void TrailInitialize()
    {
        trail.time = 0.2f;
        trail.startWidth = 0.15f;
        trail.endWidth = 0.05f;
        trail.minVertexDistance = 0.05f;

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

    void NewLauch()
    {
        collider2D.enabled = true;
        bounceTime = 0;
        endLineTriggerCount = 0;
        EnableTrail(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ===== Wall Collision Logic =====
        if (collision.gameObject.CompareTag("Wall"))
        {
            bounceTime++;
            if (bounceTime > 5)
            {
                FinishBall();
            }
        }

        // ===== BrickScript Collision Logic =====
        if (collision.gameObject.TryGetComponent<BrickScript>(out BrickScript brick))
        {
            brick.TakeDamage(1);
            bounceTime = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EndLine"))
        {
            endLineTriggerCount++;
            if (endLineTriggerCount <= maxEndLineTriggers) return;

            Vector2 newPos = new Vector2(transform.position.x, -ballManager.playScreen.squareSize * 6);
            ballManager.ResetBallPos(newPos);

            FinishBall();
        }
    }

    async void FinishBall()
    {
        EnableTrail(false);
        await ResetVelocityCoroutine();
        NewLauch();
    }

    async Task ResetVelocityCoroutine()
    {
        collider2D.enabled = false;
        rb.linearVelocity = Vector2.zero;
        await transform.DOMove(ballManager.ballPos, duration).SetEase(Ease.InOutSine)
            .AsyncWaitForCompletion();

        OnBallFinished?.Invoke(this);
    }

    public void SetProperties(Dictionary<string, float> properties)
    {
        // need per-ball state later, can cache properties here
    }

}
