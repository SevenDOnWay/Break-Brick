using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerController : MonoBehaviour {


    PlayScreen playScreen;
    BallManager ballManager;

    [SerializeField] GameObject optionPanel;

    private bool isBallMoving = false;

    LineRenderer line;
    [SerializeField] Material lineMaterial;

    List<Rigidbody2D> ballsRigidbody = new List<Rigidbody2D>();

    public event Action<Vector2> OnBallLaunch;

    [Inject]
    void Constructor(
        PlayScreen playScreen,
        BallManager ballManager
     ) {
        this.playScreen = playScreen;
        this.ballManager = ballManager;
    }

    public void StartGame() {
        SpawnLine();

        if ( optionPanel == null ) {
            Debug.LogWarning("PlayerController: Missing OptionPanel.");
        }

        if ( line == null ) {
            Debug.LogWarning("PlayerController: Missing Line.");
        }
        if ( ballManager == null ) {
            Debug.LogWarning("PlayerController: Missing ballmanger.");
        }
    }



    public void SpawnLine() {
        line = new GameObject("TrajectoryLine").AddComponent<LineRenderer>();
        line.transform.parent = transform;

        line.material = lineMaterial;

        line.startWidth = 0.1f;
        line.endWidth = 0.02f;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
            new GradientColorKey(Color.white, 0.0f),
            new GradientColorKey(Color.white, 1.0f)
            },
            new GradientAlphaKey[] {
            new GradientAlphaKey(1.0f, 0.0f),
            new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        line.colorGradient = gradient;

        line.enabled = false;
    }

    void Update() {
        if ( isBallMoving ) return;


        if ( Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) ) {
            // Convert mouse position to world
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Raycast at mouse position
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if ( optionPanel == null || ballManager == null ) return;

            // Check if we hit background
            if ( optionPanel.activeSelf ) return;
            if ( hit.collider != null && (hit.collider.CompareTag("Background") || hit.collider.CompareTag("Brick")) ) {
                // ---- Handle input only inside background ----
                if ( Input.GetMouseButtonDown(0) ) {
                    DrawLine(Input.mousePosition);
                }
                else if ( Input.GetMouseButton(0) ) {
                    DrawLine(Input.mousePosition);
                }
                else if ( Input.GetMouseButtonUp(0) ) {
                    line.enabled = false;
                    isBallMoving = true;

                    Vector2 direction = (worldPos - ballManager.ballPos).normalized;
                    OnBallLaunch?.Invoke(direction);
                }
            }
        }

    }




    void DrawLine( Vector2 pos ) {
        line.enabled = true;

        var target = Camera.main.ScreenToWorldPoint(pos);
        Vector2 direction = (target - new Vector3(ballManager.ballPos.x, ballManager.ballPos.y, 0)).normalized;


        var targetPosScreen = ballManager.ballPos + direction * Mathf.Max(Screen.width, Screen.height);

        line.positionCount = 2;
        line.SetPosition(0, ballManager.ballPos);
        line.SetPosition(1, targetPosScreen);
    }

    public void HandleAllBallsDone() {
        isBallMoving = false;
    }

    //ensure to clean up the line renderer when the object is destroyed
    void OnDestroy() {
        if ( line != null ) {
            if ( Application.isPlaying ) {
                Destroy(line.material);
                Destroy(line.gameObject);
            }
            else {
                DestroyImmediate(line.material);
                DestroyImmediate(line.gameObject);
            }
        }
    }
}