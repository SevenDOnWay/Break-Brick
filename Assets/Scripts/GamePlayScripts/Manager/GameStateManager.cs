using System;
using System.Collections;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameStateManager : MonoBehaviour, IStartable {
    WaveScript waveScript;
    PlayerController playerController;
    SpawnController spawnController;
    BallManager ballManager;
    BrickManager brickManager;
    LevelManager levelManager;

    [Inject]
    public void Constructor(
         WaveScript waveScript,
         PlayerController playerController,
         SpawnController spawnController,
         BallManager ballManager,
         BrickManager brickManager,
         LevelManager levelManager
     ) {
        this.waveScript = waveScript;
        this.playerController = playerController;
        this.spawnController = spawnController;
        this.ballManager = ballManager;
        this.brickManager = brickManager;
        this.levelManager = levelManager;
    }

    bool isPlaying = false;

    void IStartable.Start() {


        if ( ballManager == null ) {
            Debug.LogError("BallManager is null in GameStateManager.");
            return;
        }
        if ( spawnController == null ) {
            Debug.LogError("SpawnController is null in GameStateManager.");
            return;
        }
        if ( playerController == null ) {
            Debug.LogError("PlayerController is null in GameStateManager.");
            return;
        }

        ballManager.requestBall += RequestBall;
        ballManager.OnAllBallsDone += HandleAllBallsDone;
        playerController.OnBallLaunch += NotifyLaunchBall;
        levelManager.OnLevelUp += LevelUp;

        var runData = RunDataManager.Instance.runData;

        if ( runData != null && runData.isContinuing ) {
            Debug.Log("Resuming from saved RunData...");
            ContinueGame(runData);
        }
        else {
            Debug.Log("Starting a fresh run...");
            StartNewGame();
        }

        //spawnController.StartGame();
        //ballManager.StartGame();
        //playerController.StartGame();
        //brickManager.StartGame(); nothing for now
    }

    void StartNewGame() {
        spawnController.StartGame();
        ballManager.StartGame();
        playerController.StartGame();
    }

    void ContinueGame( RunData runData ) {
        // Restore bricks
        spawnController.RestoreBrick(runData.GetBricksData());
        ballManager.Restore();

        spawnController.SetUpGame();

        waveScript.SetWave(runData.GetWaveIndex());
        playerController.SpawnLine();
    }
    public void NotifyLaunchBall( Vector2 dir ) {

        if ( isPlaying ) return; // prevent multiple launch
        ballManager.LaunchBall(dir);

        isPlaying = true;

    }

    public void HandleAllBallsDone() {
        brickManager.MoveBrick();
        spawnController.SpawnBrick();
        playerController.HandleAllBallsDone();

        waveScript.IncreaseWave();

        isPlaying = false;
    }

    public GameObject RequestBall() {
        return spawnController.SpawnBall(ballManager.ballPos);
    }

    public void LevelUp() {
        StartCoroutine(LevelUpRoutine());
    }

    private IEnumerator LevelUpRoutine() {
        yield return new WaitUntil(() => !isPlaying);

        // TODO: Add upgade here

        ballManager.RequestExtraBall();
    }




}
