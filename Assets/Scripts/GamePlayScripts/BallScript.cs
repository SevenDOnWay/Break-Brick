using System.Security.Cryptography;
using UnityEngine;
using VContainer;

public class BallScript : MonoBehaviour {

    [Inject] PlayerController playerController;

    int bounceTime;
    int endLineTriggerCount = 0;
    const int maxEndLineTriggers = 2;


    void Start() {
        NewLauch();
        if ( playerController == null ) {
            Debug.LogError("PlayerController is not injected into BallScript.");
            return;
        }
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
            if ( bounceTime > 6 ) {
                transform.position = playerController.ballPos;
                bounceTime = 0;
                ResetVelocity();
            }
        }

        // ===== BrickScript Collision Logic =====
        if (collision.gameObject.TryGetComponent<BrickScript>(out BrickScript brick)){ 
            Debug.Log("BrickScript Hit");
            brick.TakeDamage(1);
            bounceTime = 0;
        }
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if ( collision.gameObject.CompareTag("EndLine")) {
            Debug.Log("EndLine Triggered");
            endLineTriggerCount++;
            if ( endLineTriggerCount <= maxEndLineTriggers ) return;
            ResetVelocity();

            playerController.ballPos = transform.position;

        }
    }

    void ResetVelocity() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

}
