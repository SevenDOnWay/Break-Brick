using System;
using UnityEngine;
using VContainer;

public class PlayerController : MonoBehaviour {

    PlayScreen playScreen;


    Vector2 ballPos;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] LineRenderer line;

    public event Action<Vector2> onLaunch;


    bool isBallMoving = false;

    [Inject]
    void Construct( PlayScreen playScreen ) {
        this.playScreen = playScreen;
    }

    void Start() {
        //TODO setup ballPos based on the game area
        ballPos = new Vector2(0, -playScreen.squareSize * 6);
        SpawnBall();
    }

    void SpawnBall() {
        Instantiate(ballPrefab, ballPos, Quaternion.identity);
    }

    void Update() {
        if ( isBallMoving ) return;

        if ( Input.GetMouseButtonDown(0) ) {
            DrawLine(Input.mousePosition);
        }
        else if ( Input.GetMouseButton(0) ) {
            DrawLine(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0) ) {
            line.enabled = false;

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchDir = (mouseWorldPos - ballPos).normalized;

            onLaunch?.Invoke(launchDir);
            //isBallMoving = true;
        }

    }

    void DrawLine( Vector2 pos ) {
        line.enabled = true;
        line.SetPosition(0, ballPos);
        line.SetPosition(1, Camera.main.ScreenToWorldPoint(pos));
    }



}
