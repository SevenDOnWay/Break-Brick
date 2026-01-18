using UnityEngine;

public class ResponseUI : MonoBehaviour {

    [SerializeField] bool updateOnRunTime;

    [SerializeField] GameObject prefab;
    [SerializeField] float padding; // Padding around the UI element


    [SerializeField] int column; // Number of columns for the UI elements
    [SerializeField] int row; // Number of rows for the UI elements

    float worldWidth;
    float worldHeight;

    float squareSize;


    private void Start() {
        // Calculate the world width and height based on the camera's orthographic size
        Camera camera = Camera.main;
        ChangeWorldSize();
        CalculateSquare();

    }

    void Update() {
        if ( updateOnRunTime ) {
            ChangeWorldSize();
            CalculateSquare();
        }
    }

    void ChangeWorldSize() {
        Camera camera = Camera.main;
        if ( camera != null ) {
            worldHeight = camera.orthographicSize * 2;
            worldWidth = worldHeight * camera.aspect;
        }
    }

    void CalculateSquare() {
        if ( worldHeight > worldWidth ) {
            squareSize = worldWidth * padding / column;
        }
        else {
            squareSize = worldHeight * padding / row;
        }

    }

    void OnDrawGizmosSelected() {
        float halfWidth = worldWidth / 2f;
        float halfHeight = worldHeight / 2f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(-halfWidth * padding, squareSize * row / 2, 0),
            new Vector3(-halfWidth * padding, -squareSize * row / 2, 0)
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(halfWidth * padding, squareSize * row / 2, 0),
            new Vector3(halfWidth * padding, -squareSize * row / 2, 0)
        );
    }



}
