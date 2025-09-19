using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BrickManager : MonoBehaviour
{


    [Inject] PlayScreen playScreen;

    const int row = 10;
    const int column = 8;

    [Inject] WaveScript waveScript;
    [SerializeField] AnimationCurve curve; //handle health of the brick


    // List<BrickScript> bricks = new List<BrickScript>();
    public BrickScript[,] bricks = new BrickScript[column, row];
    // public IReadOnlyList<BrickScript> Bricks => bricks;

    public void StartGame()
    {

    }

    public void RegisterBrick(BrickScript brick, Vector2Int pos)
    {

        // var brick = gameobject.GetComponent<BrickScript>();


        float value = curve.Evaluate(waveScript.GetWaveIndex());
        int health = Mathf.CeilToInt(value);

        brick.Init(health);

        bricks[pos.x, pos.y] = brick;
        Debug.Log($"RegisterBrick Pos: {pos}");
    }

    private void HandleBrickDestroyed(int col, int row)
    {
        bricks[col, row] = null;
    }


    public void MoveBrick()
    {
        // foreach (var brick in bricks)
        // {
        //     if (brick == null) continue; // Skip if the brick is null
        //     brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);
        //     Debug.Log(brick.transform.position);

        // }
        for (int y = row - 1; y >= 0; y--)
        {
            for (int x = column - 1; x >= 0; x--)
            {

                var brick = bricks[x, y];

                if (brick == null) continue; // Skip if the brick is null
                Debug.Log($"{x}, {y}");
                brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);
                bricks[x, y + 1] = brick;

                bricks[x, y] = null;
            }
        }
    }

    //TODO Create method handle health, and add variation to brick (brick spawn with x2 health, when die spawn smaller brick...) 

    public Vector2Int? FindBrickPosition(BrickScript target)
    {
        int cols = bricks.GetLength(0);
        int rows = bricks.GetLength(1);

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (bricks[x, y] == target)
                {
                    return new Vector2Int(x, y); // tìm thấy
                }
            }
        }
        return null; // không tìm thấy
    }
}
