using Unity.Collections;
using UnityEngine;
using VContainer;

public class BallController : MonoBehaviour {

    public struct BallData {
        public int dame;
    }

    NativeArray<BallData> ballDatas;
    //int ballCount;
    GameObject ball;
    Rigidbody2D ballRigidbody;

    [SerializeField] GameObject ballPrefab; 
    [SerializeField] float speed;

    Vector2 ballPos;

    PlayScreen playScreen;    


    [Inject]
    void Construct( PlayScreen playScreen) {
        this.playScreen = playScreen;
    }

    private void Start() {
        ballRigidbody = GetComponent<Rigidbody2D>();

        ballPos = new Vector2(0, -playScreen.squareSize * 6);
        SpawnBall();
    }

    void SpawnBall() {
        Instantiate(ballPrefab, ballPos, Quaternion.identity);
    }

    void LaunchBall( Vector2 direction ) {
        ballRigidbody.AddForce(direction * speed, ForceMode2D.Impulse); 
    }

}
