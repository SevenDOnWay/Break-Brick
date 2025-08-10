using UnityEngine;
using VContainer;

public class PlayerController : MonoBehaviour {

    PlayScreen playScreen;


    Vector2 ballPos;
    [SerializeField] GameObject ballPrefab;


    [SerializeField] LineRenderer line;

    [SerializeField] float speed = 5f;
    bool isMoving = false;

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
        if ( isMoving ) return;

        if ( Input.GetMouseButtonDown(0) ) {
            DrawLine(Input.mousePosition);
        }

    }

    void DrawLine( Vector2 pos ) {
        line.SetPosition(0, ballPos);
        line.SetPosition(1, Camera.main.ScreenToWorldPoint(pos));
    }

}
