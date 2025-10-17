using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

public class BallManager : MonoBehaviour {

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

    [SerializeField] TextMeshProUGUI t_BallCount;
    public Vector2 ballPos;
    bool ballPosLocked = false;
    [SerializeField] float xOffset = 50;
    [SerializeField] float yOffset = 20;

    CharacterSO characterSO;


    public delegate GameObject RequestBall();
    public RequestBall requestBall;
    public event Action OnAllBallsDone;




    public void StartGame() {


        ballPos = new Vector2(0, -playScreen.squareSize * 6);

        RequestExtraBall(); // init ball for play


        var data = RunDataManager.Instance.runData.GetCharacterUpgradeData();
        data.ToRuntimeSO().Apply(gameObject.GetComponent<BallManager>());

        UpdateText();
    }

    #region Upgrade_Logic

    public void RequestExtraBall( int extraballs = 1 ) {
        for ( int i = 0; i < extraballs; i++ ) {
            balls.Add(requestBall());
        }

        //TODO: Update text in here for now.

        UpdateText();
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
        //Debug.Log($"Balls in list: {balls.Count}");
    }

    IEnumerator LaunchSequence( Vector2 direction ) {
        float speed = properties["Speed"];
        //TODO: wait for done level up
        foreach ( var ball in balls ) {
            ball.GetComponent<Rigidbody2D>().AddForce(direction * speed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f); // stagger launch
        }

        yield return WaitAllBalls();
    }

    IEnumerator WaitAllBalls() {
        int finishedCount = 0;
        int totalBalls = balls.Count;
        float beginTime = Time.time;

        Action<BallScript> onBallFinished = (ball) => finishedCount++;

        // Subscribe
        foreach ( var ball in balls ) {
            var script = ball.GetComponent<BallScript>();
            script.OnBallFinished += onBallFinished;
        }

        while ( finishedCount < totalBalls ) {
            if ( Time.time > beginTime + 5f ) {
                Debug.Log("Too long, speed up balls");
                foreach ( var ball in balls ) {
                    BallScript script = ball.GetComponent<BallScript>();
                    script.rb.linearVelocity *= 3;
                }
                beginTime += 10f;
            }
            yield return null; // wait 1 frame
        }

        // Unsubscribe
        foreach ( var ball in balls ) {
            BallScript script = ball.GetComponent<BallScript>();
            script.OnBallFinished -= onBallFinished;
        }

        OnAllBallsDone?.Invoke();

        UpdateText();
        Debug.Log("All balls are done!");
    }

    void UpdateText() {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(ballPos);

        RectTransform canvasRect = t_BallCount.canvas.GetComponent<RectTransform>();
        RectTransform textRect = t_BallCount.GetComponent<RectTransform>();

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            Camera.main,
            out anchoredPos
        );

        // Now apply your offset logic in canvas space
        if ( ballPos.x > 0 )
            anchoredPos += new Vector2(-20, 150);
        else
            anchoredPos += new Vector2(20, 150);

        textRect.anchoredPosition = anchoredPos;


        t_BallCount.text = balls.Count.ToString();
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

    #region Save

    public void Save() {
        SaveBallData();
        SaveBallPos();
    }

    public void SaveBallData() {
        RunDataManager.Instance.runData.OverwriteBallCount(balls.Count);
    }

    public void SaveBallPos() {
        RunDataManager.Instance.runData.OverwriteBallPos(ballPos);
    }

    #endregion

    #region Restore

    public void Restore() {
        RestoreBallPos();
        RestoreBall();
        RestoreUpgrade();
    }

    public void RestoreBallPos() {
        ballPos = RunDataManager.Instance.runData.GetBallPos();
    }

    public void RestoreBall() {
        RequestExtraBall(RunDataManager.Instance.runData.GetBallCount());
    }

    public void RestoreUpgrade() {
        var data = RunDataManager.Instance.runData.GetCharacterUpgradeData();
        if ( data.upgradeType == UpgradeType.ExtraBalls ) {
            Debug.Log("Skip if upgrade extra ball");
            return;
        }

        data.ToRuntimeSO().Apply(gameObject.GetComponent<BallManager>());
    }
    #endregion
}
