using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BallManager : MonoBehaviour {

    SelectState selectState;
    CharacterEntry characterEntry;
    [Inject, HideInInspector] public PlayScreen playScreen;


    private Dictionary<string, float> properties = new Dictionary<string, float> {
        {"Speed", 5},
        {"CritChance", 0},
        {"CritMultiplier", 2},
        {"FireChance", 0},
        {"LightningChance", 0},

    };
    List<GameObject> balls = new List<GameObject>();

    //public IReadOnlyDictionary<string, float> GetProperties() => properties;

    public delegate GameObject RequestBall();
    public RequestBall requestBall;
    public event Action OnAllBallsDone;

    [HideInInspector] public Vector2 ballPos;
    bool ballPosLocked = false;


    public void StartGame() {

        selectState = GameObject.FindGameObjectWithTag("Select State").GetComponent<SelectState>();
        characterEntry = GameObject.FindGameObjectWithTag("Character Entry").GetComponent<CharacterEntry>();

        ballPos = new Vector2(0, -playScreen.squareSize * 6);

        RequestExtraBall();

        characterEntry.characters[selectState.characterIndex].Apply(gameObject.GetComponent<BallManager>());
    }

    #region Upgrade_Logic
    public void RequestExtraBall( int extraballs = 1 ) {
        for ( int i = 0; i < extraballs; i++ ) {
            balls.Add(requestBall());
        }
    }

    public void ModifyProperty( string key, float value ) {
        if ( !properties.ContainsKey(key) ) {
            Debug.LogWarning($"Property {key} not found!");
            return;
        }
        // propagate to all existing balls

    }
    #endregion

    public void LaunchBall( Vector2 direction ) {
        UnlockBallPos();
        StartCoroutine(LaunchSequence(direction));
        Debug.Log($"Balls in list: {balls.Count}");
    }

    IEnumerator LaunchSequence( Vector2 direction ) {
        float speed = properties["Speed"];
        foreach ( var ball in balls ) {
            ball.GetComponent<Rigidbody2D>().AddForce(direction * speed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f); // stagger launch
            Debug.Log("Ball launched with direction: " + direction);
        }

        yield return WaitAllBalls();
    }

    IEnumerator WaitAllBalls() {
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
