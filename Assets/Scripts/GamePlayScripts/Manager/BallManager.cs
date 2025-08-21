using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BallManager : MonoBehaviour {

    SelectState selectState;
    CharacterEntry characterEntry;
    PlayerController playerController;
    [Inject] IObjectResolver resolver;
    [Inject, HideInInspector] public PlayScreen playScreen;

    private Dictionary<string, float> properties = new Dictionary<string, float> {
        {"Speed", 5},
        {"CritChance", 0},
        {"CritMultiplier", 2},
        {"FireChance", 0},
        {"LightningChance", 0},

    };
    List<GameObject> balls = new List<GameObject>();

    public IReadOnlyDictionary<string, float> GetProperties() => properties;
    public IReadOnlyList<GameObject> Balls => balls;



    [HideInInspector] public Vector2 ballPos;
    bool ballPosLocked = false;




    public event Action<int> NotifyAddBall;
    public event Action OnAllBallsDone;

    private void Start() {

        selectState = GameObject.FindGameObjectWithTag("Select State").GetComponent<SelectState>();
        characterEntry = GameObject.FindGameObjectWithTag("Character Entry").GetComponent<CharacterEntry>();

        ballPos = new Vector2(0, -playScreen.squareSize * 6);

        playerController = resolver.Resolve<PlayerController>();

        //characterEntry.characters[selectState.characterIndex].Apply();



        playerController.NotifyLauchBall += LaunchBall; 
    }

    public void AddBall(int extraballs) {
        NotifyAddBall?.Invoke(extraballs);
    }
    public void RegisterBall( GameObject ball ) {
        balls.Add(ball);
    }

    public void ModifyProperty( string key, float value ) {
        if ( !properties.ContainsKey(key) ) {
            Debug.LogWarning($"Property {key} not found!");
            return;
        }


        // propagate to all existing balls

    }


    private void LaunchBall( Vector2 direction ) {
        UnlockBallPos();
        StartCoroutine(LaunchSequence(direction));
    }

    private IEnumerator LaunchSequence( Vector2 direction ) {
        float speed = properties["Speed"];
        foreach ( var ball in balls ) {
            ball.GetComponent<Rigidbody2D>().AddForce(direction * speed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f); // stagger launch
        }

        yield return WaitAllBalls();
    }

    private IEnumerator WaitAllBalls() {
        int finishedCount = 0;
        int totalBalls = balls.Count;

        Action<BallScript> onBallFinished = (ball) => finishedCount++;

        // Subscribe
        foreach ( var ball in balls ) {
            var script = ball.GetComponent<BallScript>();
            script.OnBallFinished += onBallFinished;
        }

        // Wait until all balls are finished
        yield return new WaitUntil(() => finishedCount >= totalBalls);

        // Unsubscribe
        foreach ( var ball in balls ) {
            BallScript script = ball.GetComponent<BallScript>();
            script.OnBallFinished -= onBallFinished;
        }

        OnAllBallsDone?.Invoke();
        Debug.Log("All balls are done!");
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
