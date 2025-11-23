using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameStateManager : MonoBehaviour {
    PlayScreen playScreen;
    RunDataManager runDataManager;
    WaveScript waveScript;
    PlayerController playerController;
    SpawnController spawnController;
    StatManager statManager;
    BallManager ballManager;
    BrickManager brickManager;
    LevelManager levelManager;

    [Inject]
    public void Constructor(
        PlayScreen playScreen,
        RunDataManager runDataManager,
        WaveScript waveScript,
        PlayerController playerController,
        SpawnController spawnController,
        StatManager statManager,
        BallManager ballManager,
        BrickManager brickManager,
        LevelManager levelManager
     ) {
        this.playScreen = playScreen;
        this.runDataManager = runDataManager;
        this.waveScript = waveScript;
        this.playerController = playerController;
        this.spawnController = spawnController;
        this.statManager = statManager;
        this.ballManager = ballManager;
        this.brickManager = brickManager;
        this.levelManager = levelManager;
    }

    bool isPlaying = false;

    void Start() {

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

        SetUpObserver();

        var runData = runDataManager.runData;

        //TODO: FIX THIS LOADING LOGIC

        if ( runData != null && runData.GetIsContinuing() ) {
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

    private void SetUpObserver() {
        ballManager.requestBall += RequestBall;
        ballManager.OnAllBallsDone += HandleAllBallsDone;
        playerController.OnBallLaunch += NotifyLaunchBall;
        levelManager.OnLevelUp += LevelUp;
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
        return spawnController.SpawnBall(ballManager, statManager, playScreen.GetSquareSize());
    }

    public void LevelUp() {
        //if()
        // var task = StartCoroutine(LevelUpRoutine());
    }

    private IEnumerator LevelUpRoutine() {
        yield return new WaitUntil(() => !isPlaying);

        // TODO: Add upgade here

        ballManager.RequestExtraBall();
    }




}
