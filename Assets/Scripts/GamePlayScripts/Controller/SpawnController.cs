using Unity.Hierarchy;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class SpawnController : MonoBehaviour {

    int row = 10;
    int column = 8;

    [SerializeField] GameObject prefab;
    [SerializeField] GameObject Pool;

    [SerializeReference] float padding = 0.1f;

    float squareSize ;



    void Start() {
        CalculateBrickSize();
        InitializeBrick();
    }

    void CalculateBrickSize() {
        Camera cam = Camera.main;
        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        squareSize = (worldWidth * padding) / column;

        // Apply uniform scale so prefab becomes a square
        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        float prefabOriginalWidth = sr.bounds.size.x;

        float scaleFactor = squareSize / prefabOriginalWidth;
        prefab.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

    }   

    void InitializeBrick() {
        float startX = -((column - 1) * squareSize) / 2f;
        float startY = -((row - 1) * squareSize) / 2f;

        for ( int i = 0; i < column; i++ ) {
            for ( int j = 0; j < row; j++ ) {
                Vector3 position = new Vector3(
                    startX + i * squareSize,
                    startY + j * squareSize,
                    0
                );

                GameObject brick = Instantiate(prefab, position, Quaternion.identity);
                brick.transform.SetParent(Pool.transform, true);
            }
        }

        Debug.Log($"Spawned {column * row} bricks in the pool.");
    }



}
