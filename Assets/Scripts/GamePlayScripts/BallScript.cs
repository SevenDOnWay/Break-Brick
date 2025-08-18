using System;
using System.Security.Cryptography;
using UnityEngine;
using VContainer;

public class BallScript : MonoBehaviour {

    [Inject] PlayerController playerController;

    public event Action<BallScript> OnBallFinished;

    int bounceTime;
    int endLineTriggerCount = 0;
    const int maxEndLineTriggers = 2;


    void Start() {
        NewLauch();

        playerController.OnLauchBall += NewLauch;
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

            Vector2 newPos = new Vector2(transform.position.x, -playerController.playScreen.squareSize * 6);
            playerController.ResetBallPos(newPos);

            FinishBall();
        }
    }

    void FinishBall() {
        ResetVelocityAndPosition();
        OnBallFinished?.Invoke(this); // notify controller
    }

    void ResetVelocityAndPosition() {
        transform.position = playerController.ballPos;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

}
