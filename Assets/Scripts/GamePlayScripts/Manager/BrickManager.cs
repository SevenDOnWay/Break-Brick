using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using static BrickData;

public class BrickManager : MonoBehaviour {


    [Inject] PlayScreen playScreen;
    [Inject] WaveScript waveScript;
    //[Inject] RunDataManager runDataManager;

    [SerializeField] AnimationCurve curve; //handle health of the brick
    //int row = 10;
    //int column = 8;


    float squareSize;

    List<BrickScript> bricks = new List<BrickScript>();
    public IReadOnlyList<BrickScript> Bricks => bricks;

    public void Start() {
        squareSize = playScreen.squareSize;
    }

    public void RegisterBrick( GameObject gameobject ) {

        var brick = gameobject.GetComponent<BrickScript>();


        float value = curve.Evaluate(waveScript.GetWaveIndex());
        int health = Mathf.CeilToInt(value);

        brick.Init(health);

        bricks.Add(brick);
    }

    private void HandleBrickDestroyed( BrickScript brick ) {
        bricks.Remove(brick);
    }


    public void MoveBrick() {
        foreach ( var brick in bricks ) {
            try {
                //FIX Brick null
                if ( brick == null ) throw new System.Exception("brick null");
                brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);
            }
            catch ( System.Exception e ) {
                Debug.LogError("Error moving brick position: " + e.Message);
            }

        }
    }

    //TODO: Save brick position to RunData

    //public void SaveBrick() {

    //    foreach ( var brick in bricks ) {
    //        try {
    //            if ( brick == null ) throw new System.Exception("brick null");

    //            int col = (int)(brick.transform.position.x / squareSize);
    //            int row = (int)(brick.transform.position.y / squareSize);
    //            int hp = brick.health;

    //            //TODO: check type of brick and pass to RunData
    //            BrickData.BrickType type = BrickType.Normal;

    //            if ( brick.TryGetComponent<IBrickVariant>(out var brickType) ) {
    //                type = brickType.GetBrickType();
    //            }

    //            runDataManager.runData.bricksData.Add(new BrickData(col, row, hp, type));

    //        }
    //        catch ( System.Exception e ) {
    //            Debug.LogError("Error saving brick position: " + e.Message);
    //        }
    //    }


    //}
}
