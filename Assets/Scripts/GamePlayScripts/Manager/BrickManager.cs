using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using static UnityEditor.PlayerSettings;

public class BrickManager : MonoBehaviour {


    [Inject] PlayScreen playScreen;

    const int row = 10;
    const int column = 8;

    [Inject] WaveScript waveScript;
    [SerializeField] AnimationCurve curve; //handle health of the brick


    // List<BrickScript> bricks = new List<BrickScript>();
    public BrickScript[,] bricks = new BrickScript[column, row];
    // public IReadOnlyList<BrickScript> Bricks => bricks;

    public void StartGame() {

    }

    public void RegisterBrick( BrickScript brick, Vector2Int pos ) {

        // var brick = gameobject.GetComponent<BrickScript>();


        float value = curve.Evaluate(waveScript.GetWaveIndex());
        int health = Mathf.CeilToInt(value);

        brick.Init(health);

        bricks[pos.x, pos.y] = brick;
        Debug.Log($"RegisterBrick Pos: {pos}");
    }

    private void HandleBrickDestroyed( int col, int row ) {
        bricks[col, row] = null;
    }


    public void MoveBrick() {
        // foreach (var brick in bricks)
        // {
        //     if (brick == null) continue; // Skip if the brick is null
        //     brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);
        //     Debug.Log(brick.transform.position);

        // }
        for ( int y = row - 1; y >= 0; y-- ) {
            for ( int x = column - 1; x >= 0; x-- ) {

                var brick = bricks[x, y];

                if ( brick == null ) continue; // Skip if the brick is null
                Debug.Log($"{x}, {y}");
                brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);
                bricks[x, y + 1] = brick;

                bricks[x, y] = null;
            }
        }
    }

    public void DealDamageHorizontal( Vector2Int pos ) {
        int colIndex = pos.x;
        int rowIndex = pos.y;

        for ( int x = 0; x < column; x++ ) {
            if ( x == colIndex ) continue; // skip the original brick

            var brick = bricks[x, rowIndex];
            if ( brick == null ) continue;

            brick.TakeDamage(1);
        }
    }

    public void DealDamageVertical( Vector2Int pos ) {
        int colIndex = pos.x;
        int rowIndex = pos.y;

        for ( int y = 0; y < row; y++ ) {
            if ( y == rowIndex ) continue; // skip the original brick

            var brick = bricks[colIndex, y];
            if ( brick == null ) continue;

            brick.TakeDamage(1);
        }
    }


    //TODO Create method handle health, and add variation to brick (brick spawn with x2 health, when die spawn smaller brick...) 

    //public Vector2Int? FindBrickPosition( BrickScript target ) {


    //    for ( int x = 0; x < column; x++ ) {
    //        for ( int y = 0; y < row; y++ ) {
    //            if ( bricks[x, y] == target ) {
    //                return new Vector2Int(x, y); // Found
    //            }
    //        }
    //    }
    //    return null; // Not Found
    //}


    public Vector2Int GetBrickGridIndex( Vector3 worldPos ) {
        float startX = (column - 1) * playScreen.squareSize / 2f;
        float startY = (row - 1) * playScreen.squareSize / 2f;

        int x = Mathf.RoundToInt((worldPos.x + startX) / playScreen.squareSize);
        int y = Mathf.RoundToInt((startY - worldPos.y) / playScreen.squareSize);

        Debug.Log($"{x} and {y}");

        return new Vector2Int(x, y);
    }

    public bool IsPositionOccupied( Vector2Int pos ) {
        if ( pos.x < 0 || pos.x >= column || pos.y < 0 || pos.y >= row ) {
            return true; // Out of bounds is considered occupied
        }
        return bricks[pos.x, pos.y] != null;
    }
}
