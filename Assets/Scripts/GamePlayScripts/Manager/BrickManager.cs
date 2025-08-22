using System.Collections.Generic;
using UnityEngine;
using VContainer;
using static UnityEngine.Rendering.DebugUI.Table;

public class BrickManager : MonoBehaviour {


    [Inject] PlayScreen playScreen;
    [Inject] PlayerController playerController;
    [Inject] BallManager ballManager;

    //int row = 10;
    //int column = 8;


    List<GameObject> bricks = new List<GameObject>();
    public IReadOnlyList<GameObject> Bricks => bricks;

    void Start() {
        ballManager.OnAllBallsDone += MoveBrick;
    }

    
    
    public void RegisterBrick( GameObject gameobject ) {
        bricks.Add(gameobject);
    }



    void MoveBrick() {
        Debug.Log($"Moving {bricks.Count} bricks down");
        foreach ( GameObject brick in bricks ) {
            if ( brick == null ) continue; // Skip if the brick is null
            brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);


        }
    }

    //TODO Create method handle health, and add variation to brick (brick spawn with x2 health, when die spawn smaller brick...) 


}
