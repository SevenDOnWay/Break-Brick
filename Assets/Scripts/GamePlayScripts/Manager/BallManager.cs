using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    IReadOnlyDictionary<UpgradeType, float> finalStats;


    //  [Header("Event")] //comment since header can't be use
    public event Action OnAllBallsDone;
    public delegate GameObject RequestBall();
    public RequestBall requestBall;


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
        ballPos = new Vector2(0, squareSize * -6);

        InitializeStat();

        RequestExtraBall(); // init ball for play

        //TODO: get upgrade from run data

        UpdateText();
    }

    public void InitializeStat() => finalStats = new Dictionary<UpgradeType, float>(statsManager.GetAllStats());


    #region Upgrade_Logic

    public void RequestExtraBall( int extraballs = 1 ) {
        for ( int i = 0; i < extraballs; i++ ) {
            balls.Add(requestBall());
        }

        //TODO: Update text in here for now.

        UpdateText();
    }

    //public void ModifyProperty( UpgradeType key, float value ) {

    //    if ( !baseStat.ContainsKey(key) ) {
    //        Debug.LogWarning($"Property {key} not found!");
    //        finalStat[key] = value;
    //    }

    //    finalStat[key] += value;

    //    //TODO: apply changes to all ball 


    //}
    #endregion

    //NOTE: CHECK
    public void LaunchBall( Vector2 direction ) {
        UnlockBallPos();
        StartCoroutine(LaunchSequence(direction));
        //Debug.Log($"Balls in list: {balls.Count}");
    }

    //TOOD: send dir to ball script, ball script will hold speed
    IEnumerator LaunchSequence( Vector2 direction ) {

        foreach ( var ball in balls ) {
            var script = ball.GetComponent<BallScript>();
            script.LaunchBall(direction);
            yield return new WaitForSeconds(0.1f); // stagger launch
        }

        WaitAllBalls();
    }

    void WaitAllBalls() {
        int finishedCount = 0;
        int totalBalls = balls.Count;
        float beginTime = Time.time;

        void HandleBallFinished( BallScript ball ) {
            //Unsubscribe when finished
            ball.OnBallFinished -= HandleBallFinished;

            finishedCount++;

            if ( finishedCount >= totalBalls ) AllBallDone();
        }


        if ( timeoutCoroutine != null ) StopCoroutine(timeoutCoroutine);
        timeoutCoroutine = StartCoroutine(TimeoutCheckRoutine());

        // Subscribe
        foreach ( var ball in balls ) {
            var script = ball.GetComponent<BallScript>();
            script.OnBallFinished += HandleBallFinished;
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


