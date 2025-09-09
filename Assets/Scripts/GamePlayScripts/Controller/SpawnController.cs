using System.Collections.Generic;
using System.Linq;
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
    SelectState selectstate;
    [Inject] IObjectResolver resolver;
    [Inject] PlayScreen playScreen;
    [Inject] WaveScript waveScript;
    [Inject] BrickManager brickManager;

    int row = 10;
    int column = 8;

    [System.Serializable]
    public class Brick {
        [SerializeField] public GameObject prefab;
        [SerializeField] public int minWave; //the minimum wave this brick can appear
        [SerializeField, Range(0f,1f)] public float baseChance; // base chance to spawn
        [SerializeField, Range(0f,0.1f)] public float growthPerWave; // how much chance increase per wave
    }



    //[SerializeField] float[] brickWeights;

    //[SerializeField] GameObject BrickPrefab;

    [SerializeField] Brick[] bricks;

    [SerializeField] GameObject Pool;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject BackGround;

    int difficult;

    public void StartGame() {
        ScalePrefab();
        SetUpScreen();

        selectstate = GameObject.FindGameObjectWithTag("Select State").GetComponent<SelectState>();
        difficult = selectstate.difficultyIndex;

        InitializeBrick();
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
            var TargetSquareSize = playScreen.squareSize / brickSpriteRenderer.sprite.bounds.size.x;
            brickSpriteRenderer.transform.localScale = new Vector3(TargetSquareSize, TargetSquareSize, 1f);

            if ( !brick.prefab.TryGetComponent<BoxCollider2D>(out var boxCollider) ) {
                Debug.LogError("Prefab does not have a BoxCollider2D component.");
                return;
            }
            boxCollider.size = new Vector2(TargetSquareSize, TargetSquareSize);
        }
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

    //TODO: Find solution if ball move to fast wall will not be detected
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

                GameObject BrickPrefab = GetRandomBrick();

                GameObject brick = resolver.Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.transform.SetParent(Pool.transform, true);
                brickManager.RegisterBrick(brick);
            }
        }
    }

    #endregion

    public GameObject SpawnBall( Vector2 ballPos, int extraBall = 1 ) {
        var temp = resolver.Instantiate(ballPrefab, ballPos, Quaternion.identity);
        temp.transform.position = ballPos;

        return temp;
    }

    public void SpawnBrick() {
        float startX = ((column - 1) * playScreen.squareSize) / 2f;
        float startY = ((row - 1) * playScreen.squareSize) / 2f;

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

            Vector3 position = new Vector3(
                    startX - i * playScreen.squareSize,
                    startY,
                    0
            );

            GameObject BrickPrefab = GetRandomBrick();

            GameObject brick = resolver.Instantiate(BrickPrefab, position, Quaternion.identity);
            brick.transform.SetParent(Pool.transform, true);
            brickManager.RegisterBrick(brick);
        }
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

        Debug.Log($"Eligible Bricks size: {eligibleBricks.Count}");

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



}
