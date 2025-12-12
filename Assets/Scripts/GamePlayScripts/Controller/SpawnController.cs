using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Hierarchy;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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
    IObjectResolver resolver;
    RunDataManager runDataManager;
    PlayScreen playScreen;
    WaveScript waveScript;
    BrickManager brickManager;

    [Inject]
    public void Constructor(
        IObjectResolver resolver,
        RunDataManager runDataManager,
        PlayScreen playScreen,
        WaveScript waveScript,
        BrickManager brickManager,
        UpgradeManager upgradeManager
     ) {
        this.resolver = resolver;
        this.playScreen = playScreen;
        this.waveScript = waveScript;
        this.brickManager = brickManager;
        this.runDataManager = runDataManager;
    }

    [System.Serializable]
    public class Brick {
        [SerializeField] public GameObject prefab;
        [SerializeField] public BrickType type;
        [SerializeField] public int minWave; //the minimum wave this brick can appear
        [SerializeField, Range(0f, 1f)] public float baseChance; // base chance to spawn
        [SerializeField, Range(0f, 0.1f)] public float growthPerWave; // how much chance increase per wave
    }

    public class MiniBossBrick {
        [SerializeField] public GameObject prefab;
        [SerializeField] public MiniBossBrickType type;
    }

    const int row = 10;
    const int column = 8;

    [SerializeField] Brick[] bricks;
    [SerializeField] MiniBossBrick[] miniBossBricks;


    //[SerializeField] float[] brickWeights;

    //[SerializeField] GameObject BrickPrefab;


    [SerializeField] GameObject Pool;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject BackGround;

    int difficult;
    float squareSize;

    void Start() {
        squareSize = playScreen.GetSquareSize();
    }

    public void StartGame() {
        SetUpGame();

        InitializeBrick();
    }

    public void SetUpGame() {
        ScalePrefab();
        SetUpScreen();

        difficult = runDataManager.runData.GetDifficultIndex();

    }

    #region Initialize_Screen

    void ScalePrefab() {
        if ( playScreen == null ) {
            Debug.LogError("PlayScreen is not initialized2.");
            return;
        }

        // ===== Scale Brick_Visual =====

        foreach ( var brick in bricks ) {

            var brickSpriteRenderer = brick.prefab.GetComponentInChildren<SpriteRenderer>();

            if ( brickSpriteRenderer == null ) {
                Debug.LogError("Prefab does not have a SpriteRenderer component.");
                return;
            }



            // ===== Scale BrickScript =====
            var TargetSquareSize = squareSize / brickSpriteRenderer.sprite.bounds.size.x;
            brickSpriteRenderer.transform.localScale = new Vector3(TargetSquareSize, TargetSquareSize, 1f);

            if ( !brick.prefab.TryGetComponent<BoxCollider2D>(out var boxCollider) ) {
                Debug.LogError("Prefab does not have a BoxCollider2D component.");
                return;
            }

            // Set collider size relative to sprite size (in local space)   
            Vector2 spriteSize = brickSpriteRenderer.sprite.bounds.size;
            boxCollider.size = spriteSize * TargetSquareSize;
        }
        // ===== Scale BackGround =====
        var backGroundSpriteRenderer = BackGround.GetComponent<SpriteRenderer>();

        float screenWidth = squareSize * 8;
        float screenHeight = squareSize * 11;

        // Calculate scale factor to fit both width & height
        float scaleX = screenWidth / backGroundSpriteRenderer.sprite.bounds.size.x;
        float scaleY = screenHeight / backGroundSpriteRenderer.sprite.bounds.size.y;

        BackGround.transform.localScale = new Vector3(scaleX, scaleY, 1f);

    }

    void SetUpScreen() {
        CreateBackGround();
        CreateTriggerLine();
        CreateWall("TopWall", "Wall", new Vector2(0, squareSize * 5), new Vector2(squareSize * 8, 0.1f));
        CreateWall("BottomWall", "Bottom_Wall", new Vector2(0, -squareSize * 6), new Vector2(squareSize * 8, 0.1f));
        CreateWall("LeftWall", "Wall", new Vector2(-squareSize * 4, 0), new Vector2(0.1f, squareSize * 12));
        CreateWall("RightWall", "Wall", new Vector2(squareSize * 4, 0), new Vector2(0.1f, squareSize * 12));

    }

    void CreateBackGround() {
        GameObject backGround = Instantiate(BackGround, Vector3.zero, Quaternion.identity);
        backGround.transform.parent = transform;
        backGround.tag = "Background";

        backGround.transform.position = new Vector3(0, -squareSize / 2);
    }

    void CreateTriggerLine() {
        Vector2 start = new Vector2(-squareSize * 4, -squareSize * 5);
        Vector2 end = new Vector2(squareSize * 4, -squareSize * 5);

        GameObject lineObj = new GameObject("EndLine");
        lineObj.tag = "EndLine";
        lineObj.transform.parent = transform;

        EdgeCollider2D edge = lineObj.AddComponent<EdgeCollider2D>();
        edge.isTrigger = true;
        edge.points = new Vector2[] { start, end };
    }

    //TODO: Find solution if ball move to fast wall will not be detected
    void CreateWall( string name, string tag, Vector2 position, Vector2 size ) {
        GameObject wall = new GameObject(name);
        wall.transform.parent = transform;
        wall.tag = tag;
        wall.transform.position = position;

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
        col.isTrigger = false;

        Vector2 offset = Vector2.zero;

        if ( name.Contains("Top") )
            offset = new Vector2(0, size.y / 2f);
        else if ( name.Contains("Bottom") )
            offset = new Vector2(0, -size.y / 2f);
        else if ( name.Contains("Left") )
            offset = new Vector2(-size.x / 2f, 0);
        else if ( name.Contains("Right") )
            offset = new Vector2(size.x / 2f, 0);

        wall.transform.position = position + offset;
    }

    void InitializeBrick() {

        //TODO: add a strucure to handle different difficulties

        // float startX = ((column - 1) * squareSize) / 2f;
        // float startY = ((row - 1) * squareSize) / 2f;

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
                // Vector3 position = new Vector3(startX - i * squareSize,
                //                                startY - j * squareSize,
                //                                0);
                // Debug.Log($"$InitializeBrick {i}, {j}");
                // Debug.Log($"InitializeBrick {startX - i * squareSize}, {startY - j * squareSize}");
                Vector2Int spawnPost = new(i, j);

                Vector3 worldPos = GetBrickWorldPosition(spawnPost);

                GameObject BrickPrefab = GetRandomBrick();

                GameObject brick = resolver.Instantiate(BrickPrefab, worldPos, Quaternion.identity);
                brick.name = BrickPrefab.name;
                brick.transform.SetParent(Pool.transform, true);
                brickManager.RegisterBrick(brick.GetComponent<BrickScript>(), spawnPost);
            }
        }
    }

    #endregion

    #region Restore_Data

    public void RestoreBrick( List<BrickData> bricksData ) {

        foreach ( var brickdata in bricksData ) {


            Vector2Int positionIndex = new Vector2Int(brickdata.col, brickdata.row);
            Vector3 position = GetBrickWorldPosition(positionIndex);

            GameObject brickPrefab = GetBrickPrefabFromType(brickdata.type);
            GameObject brick = resolver.Instantiate(brickPrefab, position, Quaternion.identity);
            brick.transform.SetParent(Pool.transform, true);

            brickManager.RegisterBrick(brick.GetComponent<BrickScript>(), positionIndex, brickdata.health);

        }
    }

    #endregion

    public GameObject SpawnBall( BallManager ballManager, StatManager statManager, UpgradeManager upgradeManager, float squareSize, int extraBall = 1 ) {

        Vector2 ballPos = ballManager.GetBallPos();
        var temp = resolver.Instantiate(ballPrefab, ballPos, Quaternion.identity);
        BallScript ballScript = temp.GetComponent<BallScript>();

        ballScript.Init(ballManager, statManager, upgradeManager, squareSize);

        temp.transform.position = ballPos;

        return temp;
    }

    public void SpawnBrick() {
        // float startX = ((column - 1) * squareSize) / 2f;
        // float startY = ((row - 1) * squareSize) / 2f;


        float spawnChance;
        switch ( difficult ) {
            case 0: // Easy
                spawnChance = 0.4f;
                break;
            case 1: // Normal
                spawnChance = 0.5f;
                break;
            case 2: // Hard
                spawnChance = 0.75f;
                break;
            default:
                spawnChance = 0.5f;
                break;
        }

        for ( int i = 0; i < column; i++ ) {
            if ( Random.value > spawnChance ) continue;

            // Vector3 spawnPos = new Vector3(
            //         startX - i * squareSize,
            //         startY,
            //         0
            // );
            Vector2Int spawnPos = new(i, 0);
            Vector3 worldPos = GetBrickWorldPosition(spawnPos);

            GameObject BrickPrefab = GetRandomBrick();

            GameObject brick = resolver.Instantiate(BrickPrefab, worldPos, Quaternion.identity);
            brick.name = BrickPrefab.name;
            brick.transform.SetParent(Pool.transform, true);
            brickManager.RegisterBrick(brick.GetComponent<BrickScript>(), spawnPos);
        }
    }

    /// <summary>
    /// use to spawn brick at specific position (split brick)
    /// </summary>
    /// <param name="pos"></param>
    public void SpawnBrickAt( Vector2Int pos ) {
        //Debug.Log($"SpawnBrickAt {pos}");

        if ( brickManager.IsPositionOccupied(pos) ) return; // already occupied

        Vector3 worldPos = GetBrickWorldPosition(pos);
        if ( worldPos.x > squareSize * 4 || worldPos.x < -squareSize * 4 ) return;


        // TODO: health should be half of original one
        GameObject brick = resolver.Instantiate(bricks[0].prefab, worldPos, Quaternion.identity);

        brick.transform.SetParent(Pool.transform, true);
        brickManager.RegisterBrick(brick.GetComponent<BrickScript>(), pos);
    }

    GameObject GetRandomBrick() {
        int waveIndex = waveScript.GetWaveIndex();
        var eligibleBricks = bricks.Where(b => waveIndex >= b.minWave).ToList();

        if ( eligibleBricks.Count == 0 ) {
            Debug.LogError("No eligible bricks found for the current wave index.");
            return null;
        }

        List<float> chances = new List<float>();
        foreach ( var b in eligibleBricks ) {
            float waveBonus = (waveIndex - b.minWave) * b.growthPerWave;
            chances.Add(b.baseChance + waveBonus);
        }

        // Normalize chances
        float total = chances.Sum();
        List<float> normalized = chances.Select(ch => ch / total).ToList();

        // Weighted random selection
        float rand = Random.value;
        float cumulative = 0f;
        for ( int i = 0; i < eligibleBricks.Count; i++ ) {
            cumulative += normalized[i];
            if ( rand <= cumulative ) {
                return eligibleBricks[i].prefab;
            }
        }


        // Fallback
        Debug.LogWarning("Weighted random selection failed, returning last eligible brick.");
        return eligibleBricks.Last().prefab;
    }

    public Vector3 GetBrickWorldPosition( Vector2Int pos ) {
        float startX = (column - 1) * squareSize / 2f;
        float startY = (row - 1) * squareSize / 2f;

        return new Vector3(
            -startX + pos.x * squareSize,
            startY - pos.y * squareSize,
            0
        );
    }

    public GameObject GetBrickPrefabFromType( BrickType type ) {
        foreach ( var brick in bricks ) {
            if ( brick.type == type ) {
                return brick.prefab;
            }
        }
        Debug.LogError($"Brick type {type} not found, returning default brick.");
        return bricks[0].prefab;
    }

    //public Vector2Int GetBrickGridIndex(Vector3 worldPos)
    //{
    //    float startX = (column - 1) * squareSize / 2f;
    //    float startY = (row - 1) * squareSize / 2f;

    //    int x = Mathf.RoundToInt((worldPos.x + startX) / squareSize);
    //    int y = Mathf.RoundToInt((startY - worldPos.y) / squareSize);

    //    Debug.Log($"{x} and {y}");

    //    return new Vector2Int(x, y);
    //}

}
