using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BrickManager : MonoBehaviour {


    [Inject] PlayScreen playScreen;

    //int row = 10;
    //int column = 8;
    [Inject] WaveScript waveScript;
    [SerializeField] AnimationCurve curve; //handle health of the brick
    

    List<BrickScript> bricks = new List<BrickScript>();
    public IReadOnlyList<BrickScript> Bricks => bricks;

    public void StartGame() {
        
    }    
    
    public void RegisterBrick(GameObject gameobject) {

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
            if ( brick == null ) continue; // Skip if the brick is null
            brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);

        }
    }

    //TODO Create method handle health, and add variation to brick (brick spawn with x2 health, when die spawn smaller brick...) 


}
