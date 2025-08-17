    using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;
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

    List<Rigidbody2D> ballsRigidbody = new List<Rigidbody2D>();

    public event Action OnLauchBall;



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
        foreach ( var ballRigidbody in ballsRigidbody ) {
            ballRigidbody.AddForce(direction * speed, ForceMode2D.Impulse);
        }
    }

    void DrawLine( Vector2 pos ) {
        line.enabled = true;
        var target = Camera.main.ScreenToWorldPoint(pos);
        Vector2 direction = (target - new Vector3(ballPos.x, ballPos.y, 0)).normalized;
        var targetPosScreen = ballPos + direction * Mathf.Max(Screen.width, Screen.height);
        line.SetPosition(0, ballPos);
        line.SetPosition(1, targetPosScreen);
    }

}