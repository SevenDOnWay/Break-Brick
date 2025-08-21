using UnityEngine;

public class PlayScreen {
    public float squareSize;

    public PlayScreen( int column, int row, float padding ) {

        CalculateBrickSize(column, row, padding);
    }

    void CalculateBrickSize( int column, int row, float padding ) {
        Camera cam = Camera.main;
        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        squareSize = (worldWidth * padding) / column;
    }

}
