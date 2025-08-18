using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerController : MonoBehaviour {

    [Inject] IObjectResolver resolver;
    [Inject,HideInInspector] public PlayScreen playScreen;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] float speed;
    LineRenderer line;

    [HideInInspector] public Vector2 ballPos;
    bool isBallMoving = false;
    bool ballPosLocked = false;

    List<Rigidbody2D> ballsRigidbody = new List<Rigidbody2D>();

    public event Action OnLauchBall;
    public event Action OnBallDone;



    void Start() {
        //TODO: Set up ball base on character selection

        if( playScreen == null ) {
            Debug.LogError("PlayScreen is not initialized.");
            return;
        }

        ballPos = new Vector2(0, -playScreen.squareSize * 6);
        SpawnBall();
        SpawnLine();

    }

    void SpawnBall() {
        var temp = resolver.Instantiate(ballPrefab, ballPos, Quaternion.identity);

        Rigidbody2D ballRigidbody = temp.GetComponent<Rigidbody2D>();
        ballsRigidbody.Add(ballRigidbody);
    }

    void SpawnLine() {
        line = new GameObject("Line").gameObject.AddComponent<LineRenderer>();
        line.transform.parent = transform;
        line.startWidth = 0.1f;
    }

    void Update() {
        if ( isBallMoving ) return;

        if ( Input.GetMouseButtonDown(0) ) {
            DrawLine(Input.mousePosition);
        }
        else if ( Input.GetMouseButton(0) ) {
            DrawLine(Input.mousePosition);
        }
        else if ( Input.GetMouseButtonUp(0) ) {
            line.enabled = false;
            //isBallMoving = true;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - ballPos).normalized;

            LauchBall(direction);
            OnLauchBall?.Invoke();
        }

    }

    void LauchBall( Vector2 direction ) {
        UnlockBallPos();

        foreach ( var ballRigidbody in ballsRigidbody ) {
            StartCoroutine(SuspendBall(ballRigidbody, direction));
        }

        StartCoroutine(WaitBall());
    }

    IEnumerator SuspendBall(Rigidbody2D ballRigidbody, Vector2 direction) {
        ballRigidbody.AddForce(direction * speed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator WaitBall() {
        int finishedCount = 0;
        int totalBalls = ballsRigidbody.Count;

        // Subscribe to all balls
        foreach ( var ballRigidbody in ballsRigidbody ) {
            BallScript script = ballRigidbody.GetComponent<BallScript>();
            script.OnBallFinished += ( ball ) => {
                finishedCount++;
            };
        }

        // Wait until all balls are finished
        yield return new WaitUntil(() => finishedCount == totalBalls);

        OnBallDone?.Invoke();

        Debug.Log("All balls are done!");
    }

    void DrawLine( Vector2 pos ) {
        line.enabled = true;
        var target = Camera.main.ScreenToWorldPoint(pos);
        Vector2 direction = (target - new Vector3(ballPos.x, ballPos.y, 0)).normalized;
        var targetPosScreen = ballPos + direction * Mathf.Max(Screen.width, Screen.height);
        line.SetPosition(0, ballPos);
        line.SetPosition(1, targetPosScreen);
    }

    public void ResetBallPos( Vector2 newPos ) {
        if ( !ballPosLocked ) {
            ballPos = newPos;
            ballPosLocked = true; // only first ball can update
        }
    }

    public void UnlockBallPos() {
        ballPosLocked = false;
    }
}