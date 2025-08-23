using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BrickManager : MonoBehaviour {


    [Inject] PlayScreen playScreen;
    [Inject] PlayerController playerController;
    [Inject] BallManager ballManager;

    //int row = 10;
    //int column = 8;
    int waveIndex = 0;
    [SerializeField] AnimationCurve curve; //handle health of the brick
    

    List<BrickScript> bricks = new List<BrickScript>();
    public IReadOnlyList<BrickScript> Bricks => bricks;

    public void StartGame() {
        
    }    
    
    public void RegisterBrick(GameObject gameobject) {

        var brick = gameobject.GetComponent<BrickScript>();


        float value = curve.Evaluate(waveIndex);
        int health = Mathf.CeilToInt(value);

        brick.Init(health);
        brick.OnBrickDestroyed += HandleBrickDestroyed;

        bricks.Add(brick);
    }

    private void HandleBrickDestroyed( BrickScript brick ) {
        brick.OnBrickDestroyed -= HandleBrickDestroyed;
        bricks.Remove(brick);
    }


    public void MoveBrick() {
        foreach ( var brick in bricks ) {
            if ( brick == null ) continue; // Skip if the brick is null
            brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);


        }
        waveIndex++;
    }

    //TODO Create method handle health, and add variation to brick (brick spawn with x2 health, when die spawn smaller brick...) 


}
