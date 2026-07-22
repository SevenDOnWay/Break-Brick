using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

public class BallManager : MonoBehaviour {
    RunDataManager runDataManager;
    PlayScreen playScreen;
    StatManager statsManager;
    UpgradeManager upgradeManager;



    List<GameObject> balls = new List<GameObject>();



    //TODO: Fix public access

    [Header("Ball Pos Setting")]
    bool ballPosLocked = false;
    public Vector2 ballPos;
    [SerializeField] TextMeshProUGUI t_BallCount;
    [SerializeField] float xOffset = 50;
    [SerializeField] float yOffset = 20;


    [Header("Cache property")]
    float squareSize;
    CharacterSO characterSO;
    Dictionary <UpgradeType, float> finalStats;


    //  [Header("Event")] //comment since header can't be use
    public event Action OnAllBallsDone;
    public delegate GameObject RequestBall();
    public RequestBall requestBall;
    public delegate GameObject RequestTypedBall( BallType ballType );
    public RequestTypedBall requestTypedBall;


    private Coroutine timeoutCoroutine;

    [Inject]
    public void Constructor(
        RunDataManager runDataManager,
        PlayScreen playScreen,
        StatManager stats,
        UpgradeManager upgradeManager
     ) {
        this.runDataManager = runDataManager;
        this.playScreen = playScreen;
        this.statsManager = stats;
        this.upgradeManager = upgradeManager;
    }

    public void StartGame() {

        squareSize = playScreen.squareSize;
        ballPos = new Vector2(0, squareSize * -11 / 2);

        InitializeStat();

        RequestExtraBall(); // init ball for play

        UpdateText();
    }

    public void InitializeStat() => finalStats = new Dictionary<UpgradeType, float>(statsManager.GetAllStats());

    public void RequestExtraBall( int extraballs = 1 ) {
        for ( int i = 0; i < extraballs; i++ ) {
            balls.Add(requestBall());
        }

        //TODO: Update text in here for now.

        UpdateText();
    }

    public void RequestExtraBall( BallType ballType, int extraballs = 1 ) {
        for ( int i = 0; i < extraballs; i++ ) {
            if ( ballType == BallType.Normal || requestTypedBall == null ) {
                balls.Add(requestBall());
            }
            else {
                balls.Add(requestTypedBall(ballType));
            }
        }

        UpdateText();
    }


    #region Ball_Launch_Logic
    public void LaunchBall( Vector2 direction ) {
        UnlockBallPos();
        SubscribleBall();
        StartCoroutine(LaunchSequence(direction));
        //Debug.Log($"Balls in list: {balls.Count}");
    }

    /// <summary>
    /// scribe to each ball finish event, and track when all balls are done
    /// </summary>
    void SubscribleBall() {
        int finishedCount = 0;
        int totalBalls = balls.Count;

        // Reset timeout
        if ( timeoutCoroutine != null ) StopCoroutine(timeoutCoroutine);
        timeoutCoroutine = StartCoroutine(TimeoutCheckRoutine());

        // Subscribe immediately
        foreach ( var ball in balls ) {
            var script = ball.GetComponent<BallScript>();

            // Safety check: ensure we don't double subscribe if called rapidly
            script.OnBallFinished -= HandleBallFinished;
            script.OnBallFinished += HandleBallFinished;
        }

        // Local function to handle completion
        void HandleBallFinished( BallScript ball ) {
            // Unsubscribe immediately to prevent double counting
            ball.OnBallFinished -= HandleBallFinished;

            finishedCount++;

            // Debug.Log($"Ball finished: {finishedCount}/{totalBalls}");

            if ( finishedCount >= totalBalls ) {
                AllBallDone();
            }
        }
    }

    IEnumerator LaunchSequence( Vector2 direction ) {
        foreach ( var ball in balls ) {
            var script = ball.GetComponent<BallScript>();
            script.LaunchBall(direction);
            yield return new WaitForSeconds(0.1f); // stagger launch
        }
    }

    private void AllBallDone() {
        OnAllBallsDone?.Invoke();
        UpdateText();


        StopCoroutine(timeoutCoroutine);
        timeoutCoroutine = null;

        Debug.Log("All balls are done!");
    }

    IEnumerator TimeoutCheckRoutine() {
        // Wait the initial 5 seconds
        yield return new WaitForSeconds(5f);

        Debug.Log("Too long, speed up balls");

        // Apply speed up
        foreach ( var ball in balls ) {
            BallScript script = ball.GetComponent<BallScript>();
            script.rb.linearVelocity *= 2;

        }
    }

    #endregion


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

    public Vector2 GetBallPos() => ballPos;

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
        runDataManager.runData.OverwriteBallCount(balls.Count);
    }

    public void SaveBallPos() {
        runDataManager.runData.OverwriteBallPos(ballPos);
    }

    #endregion

    #region Restore

    public void Restore() {
        RestoreBallPos();
        RestoreBall();
        RestoreUpgrade();
    }

    public void RestoreBallPos() => ballPos = runDataManager.runData.GetBallPos();

    public void RestoreBall() => RequestExtraBall(runDataManager.runData.GetBallCount());

    public void RestoreUpgrade() {



        //Dictionary<UpgradeType, float> tempStat = new Dictionary<UpgradeType, float>(baseStat);

        //foreach ( UpgradeSO upgrade in upgradeSOs ) {
        //    tempStat = upgrade.GetAllUpgradeStat();

        //    foreach ( var pair in tempStat ) {
        //        if ( pair.Key == UpgradeType.ExtraBalls ) continue;

        //        upgrade.Apply(this);

        //    }
        //}

        //TODO : read data from run data then applied to statsManager

        statsManager.ResetStats();

        //upgradeManager.RestoreUpgrades(); //not implemented yet


        //foreach ( UpgradeSO upgrade in upgradeSOs ) {
        //    statsManager.ApplyUpgrade(upgrade);
        //}

    }
    #endregion
}


