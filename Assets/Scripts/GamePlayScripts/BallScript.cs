using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using VContainer;

public class BallScript : MonoBehaviour {

    [Inject] BallManager ballManager;


    [HideInInspector] public Rigidbody2D rb;
    int bounceTime;
    int endLineTriggerCount = 0;
    const int maxEndLineTriggers = 2;
    float duration = 0.5f; // duration for moving back to start position

    public event Action<BallScript> OnBallFinished;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        NewLauch();

    }

    void NewLauch() {
        bounceTime = 0;
        endLineTriggerCount = 0;
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

            Vector2 newPos = new Vector2(transform.position.x, -ballManager.playScreen.squareSize * 6);
            ballManager.ResetBallPos(newPos);

            FinishBall();
        }
    }

    void FinishBall() {
        NewLauch();
        StartCoroutine(ResetVelocityCoroutine());
    }

    IEnumerator ResetVelocityCoroutine() {
        rb.linearVelocity = Vector2.zero;
        yield return transform.DOMove(ballManager.ballPos, duration).WaitForCompletion();
        OnBallFinished?.Invoke(this);
    }

    public void SetProperties( Dictionary<string, float> properties ) {
        // need per-ball state later, can cache properties here
    }

}
