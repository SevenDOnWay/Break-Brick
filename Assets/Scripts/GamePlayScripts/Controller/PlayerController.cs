using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;
using VContainer.Unity;

public class PlayerController : MonoBehaviour {

    [Inject] PlayScreen playScreen;
    [Inject] IObjectResolver resolver;  

    public Vector2 ballPos;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] float speed;
    [SerializeField] LineRenderer line;
    [SerializeField] float ballRaycastRadius = 0.5f; // Radius of the ball for raycasting

    List<Rigidbody2D> ballsRigidbody = new List<Rigidbody2D>();
    public event Action OnLauchBall;

    bool isBallMoving = false;


    void Start()
    {
        //TODO setup ballPos based on the game area

        if ( playScreen == null ) {
            Debug.LogError("PlayScreen is not injected into PlayerController.");
            return;
        }

        ballPos = new Vector2(0, -playScreen.squareSize * 6);
        SpawnBall();
    }

    void SpawnBall() {
        var temp = resolver.Instantiate(ballPrefab, ballPos, Quaternion.identity);

        Rigidbody2D ballRigidbody = temp.GetComponent<Rigidbody2D>();
        ballsRigidbody.Add(ballRigidbody);

    }

    void Update()
    {
        if (isBallMoving) return;

        if (Input.GetMouseButtonDown(0))
        {
            DrawLine(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            DrawLine(Input.mousePosition);
        }
        else if ( Input.GetMouseButtonUp(0) ) {
            line.enabled = false;
            isBallMoving = true;

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

    void DrawLine(Vector2 pos)
    {
        line.enabled = true;
        line.SetPosition(0, ballPos);
        line.SetPosition(1, Camera.main.ScreenToWorldPoint(pos));
    }

}