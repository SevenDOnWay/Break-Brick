using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    GameOverScript gameOverScript;

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
        UpgradeManager upgradeManager,
        GameOverScript gameOverScript
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
        this.gameOverScript = gameOverScript;
    }

    bool isBallsFlying = false;
    bool isUpgrading = false;
    bool isGameOver = false;

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
        ballManager.requestTypedBall += RequestBall;
        playerController.OnBallLaunch += NotifyLaunchBall;
        ballManager.OnAllBallsDone += HandleAllBallsDone;
        levelManager.NotifiLevelUp += LevelUp;
        upgradeManager.OnAllUpgradesProcessed += FinishUpgrade;
        upgradeManager.RequestExtraBalls += (ballType, extraballs) => ballManager.RequestExtraBall(ballType, extraballs);
        brickManager.GameOverEvent += CallGameOver;
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
        brickManager.HandleAllBallDone();
        spawnController.SpawnBrick();
        //playerController.HandleAllBallsDone();

        waveScript.IncreaseWave();

        isBallsFlying = false;

        if(waveScript.GetWaveIndex() % 50 == 0) {
            spawnController.SpawnBoss();
        }

        CheckTurnState();
    }

    public GameObject RequestBall() {
        return spawnController.SpawnBall(ballManager, statManager, upgradeManager, playScreen.GetSquareSize());
    }

    public GameObject RequestBall( BallType ballType ) {
        return spawnController.SpawnBall(ballManager, statManager, upgradeManager, playScreen.GetSquareSize(), ballType);
    }

    public void LevelUp( int currentLevel ) {
        if( isGameOver) return;

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
        if ( !isBallsFlying && !isUpgrading && !isGameOver) {
            SetPlayerCanShoot(true);
        }
        else {
            SetPlayerCanShoot(false);
        }
    }

    private void SetPlayerCanShoot( bool canShoot ) {
        playerController.SetCanShoot(canShoot);
    }

    private void CallGameOver() {
        _ = CallGameOverAsync();
    }

    private async Task CallGameOverAsync() {
        isGameOver = true;
        SetPlayerCanShoot(false); // prevent player from shooting when game is already over

        //add observable event for game over and subscribe game over script to it,
        //then invoke it here instead of directly calling game over script,
        //this will make the code more decoupled and easier to manage in the long run

        //TODO: add option to revive 
        await gameOverScript.HandleGameOver();

        runDataManager.DeleteRun();

        await SceneManager.LoadSceneAsync(1); 

    }
}
