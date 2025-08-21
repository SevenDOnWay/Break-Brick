using UnityEngine;

public class PlayScreen {
    public float squareSize;

    public PlayScreen( Camera camera, int column, int row, float padding ) {

        CalculateBrickSize(camera, column, row, padding);
    }

    void CalculateBrickSize( Camera camera, int column, int row, float padding ) {
        float worldHeight = camera.orthographicSize * 2f;
        float worldWidth = worldHeight * camera.aspect;

        squareSize = (worldWidth * padding) / column;
    }

}
