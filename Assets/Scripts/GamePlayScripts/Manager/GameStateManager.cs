using System;
using System.Collections;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameStateManager : MonoBehaviour, IStartable {
    [Inject] PlayerController playerController;
    [Inject] SpawnController spawnController;
    [Inject] BallManager ballManager;
    [Inject] BrickManager brickManager;
    [Inject] LevelManager levelManager;

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

        spawnController.StartGame();
        ballManager.StartGame();
        playerController.StartGame();
        //brickManager.StartGame(); nothing for now
    }
    public void NotifyLaunchBall( Vector2 dir ) {
        ballManager.LaunchBall(dir);

        isPlaying = true;
    }

    public void HandleAllBallsDone() {
        brickManager.MoveBrick();
        spawnController.SpawnBrick();
        playerController.HandleAllBallsDone();

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
