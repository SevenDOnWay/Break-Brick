using Unity.Collections;
using UnityEngine;
using VContainer;

public class BallController : MonoBehaviour {

    public struct BallData {
        public int dame;
    }

    NativeArray<BallData> ballDatas;
    //int ballCount;
    Rigidbody2D ballRigidbody;
    [SerializeField] float speed;

    PlayerController playerController;

    [Inject]
    void Construct( PlayerController playerController ) {
        this.playerController = playerController;
    }

    private void Start() {
        ballRigidbody = GetComponent<Rigidbody2D>();
        playerController.onLaunch += LaunchBall;
    }

    void Update() {

    }

    void LaunchBall( Vector2 direction ) {
        ballRigidbody.AddForce(direction * speed, ForceMode2D.Impulse); 
    }

}
