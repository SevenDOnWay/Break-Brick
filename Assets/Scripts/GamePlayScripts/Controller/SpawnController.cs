using Unity.Hierarchy;
using UnityEngine;
using VContainer;
using static UnityEngine.Rendering.DebugUI.Table;

public class SpawnController : MonoBehaviour {

    [SerializeField] int row = 10;
    [SerializeField] int column = 8;

    [SerializeField] GameObject BrickPrefab;
    [SerializeField] GameObject Pool;

    PlayScreen playScreen;

    [Inject]
    void Construct( PlayScreen playScreen ) {
        this.playScreen = playScreen;
    }

    void Start()
    {
        ScalePrefab();
        SetUpScreen();
        InitializeBrick();

    }

    void ScalePrefab()
    {
        // ===== Scale Brick_Visual =====
        var spriteRenderer = BrickPrefab.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Prefab does not have a SpriteRenderer component.");
            return;
        }

        var TargetSize = playScreen.squareSize / spriteRenderer.sprite.bounds.size.x;
        spriteRenderer.transform.localScale = new Vector3(TargetSize, TargetSize, 1f);

        // ===== Scale BrickScript Raycast =====
        if (!BrickPrefab.TryGetComponent<BoxCollider2D>(out var boxCollider))
        {
            Debug.LogError("Prefab does not have a BoxCollider2D component.");
            return;
        }
        boxCollider.size = new Vector2(TargetSize, TargetSize);
    }

    void SetUpScreen() {
        CreateTriggerLine();
        CreateWall("TopWall", new Vector2(0, playScreen.squareSize * 5), new Vector2(playScreen.squareSize * 8, 0.1f));
        CreateWall("BottomWall", new Vector2(0, -playScreen.squareSize * 6), new Vector2(playScreen.squareSize * 8, 0.1f));
        CreateWall("LeftWall", new Vector2(-playScreen.squareSize * 4, 0), new Vector2(0.1f, playScreen.squareSize * 12));
        CreateWall("RightWall", new Vector2(playScreen.squareSize * 4, 0), new Vector2(0.1f, playScreen.squareSize * 12));
    }
    void CreateTriggerLine() {
        Vector2 start = new Vector2(-playScreen.squareSize * 4, -playScreen.squareSize * 5);
        Vector2 end = new Vector2(playScreen.squareSize * 4, -playScreen.squareSize * 5);

        GameObject lineObj = new GameObject("EndLine");
        lineObj.tag = "EndLine";
        lineObj.transform.parent = transform;

        EdgeCollider2D edge = lineObj.AddComponent<EdgeCollider2D>();
        edge.isTrigger = true;
        edge.points = new Vector2[] { start, end };
    }

    void CreateWall( string name, Vector2 position, Vector2 size ) {
        GameObject wall = new GameObject(name);
        wall.transform.parent = transform;
        wall.tag = "Wall";
        wall.transform.position = position;

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
        col.isTrigger = false;

        Debug.Log($"Created wall: {name} at position {position} with size {size}");
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

                GameObject brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.transform.SetParent(Pool.transform, true);
            }
        }

        Debug.Log($"Spawned {column * row} bricks in the pool.");
    }

}
