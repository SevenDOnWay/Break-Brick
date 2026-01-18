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
    UpgradeManager upgradeManager;

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
        LevelManager levelManager,
        UpgradeManager upgradeManager
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
        this.upgradeManager = upgradeManager;
    }

    bool isBallsFlying = false;
    bool isUpgrading = false;

    void Start() {

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

    }

    private void SetUpObserver() {
        ballManager.requestBall += RequestBall;
        playerController.OnBallLaunch += NotifyLaunchBall;
        ballManager.OnAllBallsDone += HandleAllBallsDone;
        levelManager.NotifiLevelUp += LevelUp;
        upgradeManager.OnAllUpgradesProcessed += FinishUpgrade;
        upgradeManager.RequestExtraBalls += (extraballs) => ballManager.RequestExtraBall(extraballs);
    }

    void StartNewGame() {
        spawnController.StartGame(); 
        ballManager.StartGame();
        playerController.StartGame();
        upgradeManager.StartGame();

        SetPlayerCanShoot(true);
    }

    void ContinueGame( RunData runData ) {

        spawnController.RestoreBrick(runData.GetBricksData());
        ballManager.Restore();

        spawnController.SetUpGame();

        waveScript.SetWave(runData.GetWaveIndex());
        playerController.SpawnLine();
    }

    public void NotifyLaunchBall( Vector2 dir ) {

        if ( isBallsFlying ) return; // prevent multiple launch

        ballManager.LaunchBall(dir);

        isBallsFlying = true;

    }

    public void HandleAllBallsDone() {
        brickManager.MoveBrick();
        spawnController.SpawnBrick();
        //playerController.HandleAllBallsDone();

        waveScript.IncreaseWave();

        isBallsFlying = false;

        if(waveScript.GetWaveIndex() % 50 == 0) {
            spawnController.SpawnMiniBoss();
        }

        CheckTurnState();
    }

    public GameObject RequestBall() {
        return spawnController.SpawnBall(ballManager, statManager, upgradeManager, playScreen.GetSquareSize());
    }

    public void LevelUp( int currentLevel ) {
        StartCoroutine(LevelUpRoutine(currentLevel));
    }

    private IEnumerator LevelUpRoutine(int currentLevel) {
        yield return new WaitWhile(() => isBallsFlying);

        isUpgrading = true;
        SetPlayerCanShoot(false);

        upgradeManager.SetUpUpgrade(currentLevel);

        ballManager.RequestExtraBall();
    }


    public void FinishUpgrade() {
        isUpgrading = false;
        CheckTurnState();
    }

    private void CheckTurnState() {
        // Only allow shooting if balls are NOT flying AND we are NOT upgrading
        if ( !isBallsFlying && !isUpgrading ) {
            SetPlayerCanShoot(true);
        }
        else {
            SetPlayerCanShoot(false);
        }
    }

    private void SetPlayerCanShoot( bool canShoot ) {
        playerController.SetCanShoot(canShoot);
    }
}
