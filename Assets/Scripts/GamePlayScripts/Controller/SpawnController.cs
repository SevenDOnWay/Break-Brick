using Unity.Hierarchy;
using UnityEngine;
using VContainer;
using static UnityEngine.Rendering.DebugUI.Table;

public class SpawnController : MonoBehaviour {

    int row = 10;
    int column = 8;

    [SerializeField] GameObject prefab;
    [SerializeField] GameObject Pool;

    PlayScreen playScreen;

    [Inject]
    void Construct( PlayScreen playScreen ) {
        this.playScreen = playScreen;
    }


    void Start() {
        InitializeBrick();
    }  

    void InitializeBrick() {
        float startX = -((column - 1) * playScreen.squareSize) / 2f;
        float startY = -((row - 1) * playScreen.squareSize) / 2f;

        for ( int i = 0; i < column; i++ ) {
            for ( int j = 0; j < row; j++ ) {
                Vector3 position = new Vector3(
                    startX + i * playScreen.squareSize,
                    startY + j * playScreen.squareSize,
                    0
                );

                GameObject brick = Instantiate(prefab, position, Quaternion.identity);
                brick.transform.SetParent(Pool.transform, true);
            }
        }

        Debug.Log($"Spawned {column * row} bricks in the pool.");
    }




}
