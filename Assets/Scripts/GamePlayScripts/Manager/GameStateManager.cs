using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameStateManager : MonoBehaviour, IStartable {
    [Inject] PlayerController playerController;
    [Inject] SpawnController spawnController;
    [Inject] BallManager ballManager;
    [Inject] BrickManager brickManager;
    [Inject] LevelManager levelManager;


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
    public void HandleAllBallsDone() {
        brickManager.MoveBrick();
        spawnController.SpawnBrick();
        playerController.HandleAllBallsDone();
    }

    public void NotifyLaunchBall( Vector2 dir ) {
        ballManager.LaunchBall(dir);
    }


    public GameObject RequestBall() {
        return spawnController.SpawnBall(ballManager.ballPos);
    }

    public void LevelUp() {
        //TODO: Add level up logic here +1 upgrade.

        ballManager.RequestExtraBall();


    }




}
