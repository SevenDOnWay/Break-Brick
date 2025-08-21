using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerController : MonoBehaviour {


    [Inject,HideInInspector] PlayScreen playScreen;
    [Inject] BallManager ballManager;

    private bool isBallMoving = false;

    [SerializeField] float speed;
    LineRenderer line;




    List<Rigidbody2D> ballsRigidbody = new List<Rigidbody2D>();

    public event Action<Vector2> NotifyLauchBall;




    void Start() {
        //TODO: Set up ball base on character selection

        if ( playScreen == null ) {
            Debug.LogError("PlayScreen is not initialized1.  ");
            return;
        }

        SpawnLine();
        ballManager.OnAllBallsDone += HandleAllBallsDone;

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
            Vector2 direction = (mousePos - ballManager.ballPos).normalized;

            NotifyLauchBall?.Invoke(direction);
        }

    }




    void DrawLine( Vector2 pos ) {
        line.enabled = true;
        var target = Camera.main.ScreenToWorldPoint(pos);
        Vector2 direction = (target - new Vector3(ballManager.ballPos.x, ballManager.ballPos.y, 0)).normalized;
        var targetPosScreen = ballManager.ballPos + direction * Mathf.Max(Screen.width, Screen.height);
        line.SetPosition(0, ballManager.ballPos);
        line.SetPosition(1, targetPosScreen);
    }

    private void HandleAllBallsDone() {
        isBallMoving = false;
    }

}