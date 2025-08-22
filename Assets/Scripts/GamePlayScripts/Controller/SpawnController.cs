using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using VContainer;
using VContainer.Unity;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;

public class SpawnController : MonoBehaviour {

    /*
     * enum Difficulties for the game.
     * 
    enum Difficult{
        Easy = 0,
        Normal = 1,
        Hard = 2
    }

    */
    [Inject] IObjectResolver resolver;
    [Inject] PlayScreen playScreen;
    [Inject] PlayerController playerController;
    [Inject] BallManager ballManager;
    [Inject] BrickManager brickManager;
    SelectState selectstate;

    int row = 10;
    int column = 8;

    [SerializeField] GameObject BrickPrefab;
    [SerializeField] GameObject Pool;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject BackGround;

    int difficult;


    void Start() {
        ScalePrefab();
        SetUpScreen();

        selectstate = GameObject.FindGameObjectWithTag("Select State").GetComponent<SelectState>();
        difficult = selectstate.difficultyIndex;

        InitializeBrick();

        SpawnBall(1); // Spawn initial ball

        ballManager.NotifyAddBall += SpawnBall;
        ballManager.OnAllBallsDone += SpawnBrick;
    }

    #region initialize_Screen

    void ScalePrefab() {
        // ===== Scale Brick_Visual =====
        var brickSpriteRenderer = BrickPrefab.GetComponentInChildren<SpriteRenderer>();

        if ( brickSpriteRenderer == null ) {
            Debug.LogError("Prefab does not have a SpriteRenderer component.");
            return;
        }

        if ( playScreen == null ) {
            Debug.LogError("PlayScreen is not initialized2.");
            return;
        }

        // ===== Scale BrickScript =====
        var TargetSquareSize = playScreen.squareSize / brickSpriteRenderer.sprite.bounds.size.x;
        brickSpriteRenderer.transform.localScale = new Vector3(TargetSquareSize, TargetSquareSize, 1f);

        if ( !BrickPrefab.TryGetComponent<BoxCollider2D>(out var boxCollider) ) {
            Debug.LogError("Prefab does not have a BoxCollider2D component.");
            return;
        }
        boxCollider.size = new Vector2(TargetSquareSize, TargetSquareSize);

        // ===== Scale BackGround =====
        var backGroundSpriteRenderer = BackGround.GetComponent<SpriteRenderer>();

        float screenWidth = playScreen.squareSize * 8;
        float screenHeight = playScreen.squareSize * 11;

        // Calculate scale factor to fit both width & height
        float scaleX = screenWidth / backGroundSpriteRenderer.sprite.bounds.size.x;
        float scaleY = screenHeight / backGroundSpriteRenderer.sprite.bounds.size.y;

        BackGround.transform.localScale = new Vector3(scaleX, scaleY, 1f);

    }

    void SetUpScreen() {
        CreateBackGround();
        CreateTriggerLine();
        CreateWall("TopWall", new Vector2(0, playScreen.squareSize * 5), new Vector2(playScreen.squareSize * 8, 0.1f));
        CreateWall("BottomWall", new Vector2(0, -playScreen.squareSize * 6), new Vector2(playScreen.squareSize * 8, 0.1f));
        CreateWall("LeftWall", new Vector2(-playScreen.squareSize * 4, 0), new Vector2(0.1f, playScreen.squareSize * 12));
        CreateWall("RightWall", new Vector2(playScreen.squareSize * 4, 0), new Vector2(0.1f, playScreen.squareSize * 12));

    }

    void CreateBackGround() {
        GameObject backGround = Instantiate(BackGround, Vector3.zero, Quaternion.identity);
        backGround.transform.parent = transform;
        backGround.tag = "BackGround";

        backGround.transform.position = new Vector3(0, -playScreen.squareSize / 2);
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
    }

    void InitializeBrick() {

        //TODO: add a strucure to handle different difficulties

        float startX = ((column - 1) * playScreen.squareSize) / 2f;
        float startY = ((row - 1) * playScreen.squareSize) / 2f;

        int target;
        float spawnChance;

        switch ( difficult ) {
            case 0:
                target = 3;
                spawnChance = 0.5f;
                break;
            case 1: // Normal
                target = 3;
                spawnChance = 0.75f;
                break;
            case 2: // Hard
                target = 4;
                spawnChance = 0.9f;
                break;
            default:
                Debug.LogError("Invalid difficulty level. Defaulting to Normal.");
                target = 3; // Default to Normal if invalid difficulty
                spawnChance = 0.75f;
                break;
        }

        for ( int i = 0; i < column; i++ ) {
            for ( int j = 0; j < target; j++ ) {
                if ( Random.value > spawnChance ) continue;
                Vector3 position = new Vector3(startX - i * playScreen.squareSize,
                                               startY - j * playScreen.squareSize,
                                               0);

                GameObject brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.transform.SetParent(Pool.transform, true);
                brickManager.RegisterBrick(brick);
            }
        }
    }

    #endregion

    void SpawnBall( int extraballs ) {
        for ( int i = 0; i < extraballs; i++ ) {
            var temp = resolver.Instantiate(ballPrefab, ballManager.ballPos, Quaternion.identity);
            temp.transform.position = ballManager.ballPos;

            ballManager.RegisterBall(temp);
        }
    }
    void SpawnBrick() {
        float startX = ((column - 1) * playScreen.squareSize) / 2f;
        float startY = ((row - 1) * playScreen.squareSize) / 2f;

        float spawnChance;
        switch ( difficult ) {
            case 0: // Easy
                spawnChance = 0.5f;
                break;
            case 1: // Normal
                spawnChance = 0.75f;
                break;
            case 2: // Hard
                spawnChance = 0.9f;
                break;
            default:
                spawnChance = 0.75f;
                break;
        }

        for ( int i = 0; i < column; i++ ) {
            if ( Random.value > spawnChance ) continue;
            Vector3 position = new Vector3(
                    startX - i * playScreen.squareSize,
                    startY,
                    0
                );

            GameObject brick = Instantiate(BrickPrefab, position, Quaternion.identity);
            brick.transform.SetParent(Pool.transform, true);
            brickManager.RegisterBrick(brick);
        }
    }



    //TODO: move to brickmanager


}
