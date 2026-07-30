using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SpawnController : MonoBehaviour {

    /*
    enum Difficult{
        Easy = 0,
        Normal = 1,
        Hard = 2
    }
    */

    const int row = 10;
    const int column = 8;
    const string SpecialBallConfigResourcePath = "SpecialBalls/{0}BallConfig";

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
        BrickManager brickManager
     ) {
        this.resolver = resolver;
        this.playScreen = playScreen;
        this.waveScript = waveScript;
        this.brickManager = brickManager;
        this.runDataManager = runDataManager;
    }

    [System.Serializable]
    public class Brick {
        [SerializeField] public BrickType type;
        [SerializeField] public GameObject prefab;
        [SerializeField] public int minWave; //the minimum wave this brick can appear
        [SerializeField, Min(0.1f)] public float budgetCost = 1f;
        [SerializeField, Range(0f, 1f)] public float baseChance; // relative selection weight
        [SerializeField, Range(0f, 0.1f)] public float growthPerWave; // selection-weight growth per wave

        [SerializeField] public bool isMiniBoss;
        [SerializeField] public bool isBoss;
    }

    [SerializeField] Brick[] bricks;
    [SerializeField] GameObject Pool;

    [Header("Wave Budget")]
    [Tooltip("Budget available for a normal-difficulty wave before difficulty scaling.")]
    [SerializeField, Min(0.1f)] float baseWaveBudget = 6f;
    [SerializeField, Min(0f)] float budgetGrowthPerWave = 0.5f;
    [Tooltip("Initial board receives this many normal wave budgets so it can fill several rows.")]
    [SerializeField, Min(1f)] float initialWaveBudgetMultiplier = 3f;
    [SerializeField, Min(1)] int initialRows = 3;
    [SerializeField, Min(1)] int hardInitialRows = 4;
    [SerializeField, Min(0.1f)] float easyBudgetMultiplier = 0.67f;
    [SerializeField, Min(0.1f)] float normalBudgetMultiplier = 1f;
    [SerializeField, Min(0.1f)] float hardBudgetMultiplier = 1.2f;

    [SerializeField] LayerMask wallLayer;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] SpecialBallConfig normalBallConfig;
    [SerializeField] BallPrefabBinding[] specialBallPrefabs;
    [SerializeField] GameObject BackGround;

    int difficult;
    float squareSize;

    [System.Serializable]
    public class BallPrefabBinding {
        [SerializeField] public BallType type;
        [SerializeField] public GameObject prefab;
        [SerializeField] public SpecialBallConfig config;
    }


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
            if ( brick == null || brick.prefab == null ) {
                Debug.LogWarning("Skipping a brick definition because its prefab is not configured.");
                continue;
            }

            // Bosses use their own visual setup and are not SpriteRenderer-based.
            if ( brick.isBoss ) {
                continue;
            }


            var brickSpriteRenderer = brick.prefab
                .GetComponentsInChildren<SpriteRenderer>(true)
                .FirstOrDefault(renderer => renderer.sprite != null);

            if ( brickSpriteRenderer == null ) {
                Debug.LogWarning($"Skipping {brick.prefab.name}: no SpriteRenderer with a sprite was found.");
                continue;
            }

            if ( !brick.prefab.TryGetComponent<BoxCollider2D>(out var boxCollider) ) {
                Debug.LogError("Prefab does not have a BoxCollider2D component.");
                return;
            }

            float targetSquareSize = squareSize / brickSpriteRenderer.sprite.bounds.size.x;

            if ( brick.isBoss ) {
                //ScalePrefabBoss(brickSpriteRenderer);

                brickSpriteRenderer.transform.localScale = new Vector3(targetSquareSize * 8, targetSquareSize * 3, 1f);

            }

            // ===== Scale BrickScript =====
            brickSpriteRenderer.transform.localScale = new Vector3(targetSquareSize, targetSquareSize, 1f);



            // Set collider size relative to sprite size (in local space)   
            Vector2 spriteSize = brickSpriteRenderer.sprite.bounds.size;
            boxCollider.size = spriteSize * targetSquareSize;

            // ===== Scale Effect Layer =====
            var brickScript = brick.prefab.GetComponent<BrickScript>();
            if ( brickScript != null ) {
                foreach ( var binding in brickScript.effectLayerBindings ) {
                    if ( binding.layerObject != null ) {
                        // Scale the effect layer to match the visual scale
                        binding.layerObject.transform.localScale = new Vector3(targetSquareSize, targetSquareSize, 1f);
                    }
                }
            }
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

    void ScalePrefabBoss() {

    }

    void SetUpScreen() {
        CreateBackGround();
        CreateTriggerLine();
        CreateWall("TopWall", "Wall", wallLayer, new Vector2(0, squareSize * 5), new Vector2(squareSize * 8, 0.1f));
        CreateWall("BottomWall", "Bottom_Wall", wallLayer, new Vector2(0, -squareSize * 6), new Vector2(squareSize * 8, 0.1f));
        CreateWall("LeftWall", "Wall", wallLayer, new Vector2(-squareSize * 4, 0), new Vector2(0.1f, squareSize * 12));
        CreateWall("RightWall", "Wall", wallLayer, new Vector2(squareSize * 4, 0), new Vector2(0.1f, squareSize * 12));

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
    void CreateWall( string name, string tag, LayerMask mask, Vector2 position, Vector2 size ) {
        GameObject wall = new GameObject(name);
        wall.transform.parent = transform;
        wall.tag = tag;
        wall.transform.position = position;
        wall.layer = mask;

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
        int rowCount = difficult == 2 ? hardInitialRows : initialRows;
        int startRow = brickManager.NormalSpawnStartRow;
        rowCount = Mathf.Min(rowCount, row - startRow);
        var positions = new List<Vector2Int>(column * rowCount);

        for ( int x = 0; x < column; x++ ) {
            for ( int y = 0; y < rowCount; y++ ) {
                positions.Add(new Vector2Int(x, startRow + y));
            }
        }

        SpawnBricksForBudget(positions, GetCurrentWaveBudget() * initialWaveBudgetMultiplier);
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

    public GameObject SpawnBall( BallManager ballManager, StatManager statManager, float squareSize, int extraBall = 1 ) {
        return SpawnBall(ballManager, statManager, squareSize, BallType.Normal, extraBall);
    }

    public GameObject SpawnBall(
        BallManager ballManager,
        StatManager statManager,
        float squareSize,
        BallType ballType,
        int extraBall = 1
    ) {

        Vector2 ballPos = ballManager.GetBallPos();
        BallPrefabBinding binding = GetBallBinding(ballType);
        GameObject prefab = binding?.prefab != null ? binding.prefab : ballPrefab;
        SpecialBallConfig config = GetBallConfig(ballType, binding);

        var temp = resolver.Instantiate(prefab, ballPos, Quaternion.identity);
        BallScript ballScript = temp.GetComponent<BallScript>();

        ballScript.Init(ballManager, statManager, squareSize);
        ballScript.SetSpecialBallConfig(config);

        temp.transform.position = ballPos;

        return temp;
    }

    BallPrefabBinding GetBallBinding( BallType ballType ) {
        if ( ballType == BallType.Normal || specialBallPrefabs == null ) {
            return null;
        }

        foreach ( var binding in specialBallPrefabs ) {
            if ( binding != null && binding.type == ballType ) {
                return binding;
            }
        }

        Debug.LogWarning($"Special ball prefab for {ballType} is not configured. Using the default ball prefab.");
        return null;
    }

    SpecialBallConfig GetBallConfig( BallType ballType, BallPrefabBinding binding ) {
        if ( ballType == BallType.Normal ) {
            return normalBallConfig;
        }

        if ( binding?.config != null ) {
            return binding.config;
        }

        string resourcePath = string.Format(SpecialBallConfigResourcePath, ballType);
        SpecialBallConfig resourceConfig = Resources.Load<SpecialBallConfig>(resourcePath);
        if ( resourceConfig != null ) {
            return resourceConfig;
        }

        Debug.LogWarning($"Special ball config not found at Resources/{resourcePath}. Falling back to normal ball config.");
        return normalBallConfig;
    }

    public void SpawnBrick() {
        int spawnRow = brickManager.NormalSpawnStartRow;
        if ( spawnRow >= row ) return;

        var positions = new List<Vector2Int>(column);
        for ( int x = 0; x < column; x++ ) {
            positions.Add(new Vector2Int(x, spawnRow));
        }

        SpawnBricksForBudget(positions, GetCurrentWaveBudget());
    }

    public void SpawnMiniBoss() {

        int x = Random.Range(0, column);

        Vector2Int spawnPos = new(x, 0);

        if ( brickManager.IsPositionOccupied(spawnPos) ) OverideBrick(spawnPos);


        //GameObject brick = resolver.Instantiate(BrickPrefab, spawnPos, Quaternion.identity);
        //brick.name = BrickPrefab.name;
        //brick.transform.SetParent(Pool.transform, true);
        //brickManager.RegisterBrick(brick.GetComponent<BrickScript>(), spawnPos);

    }

    public void SpawnBoss() {
        if ( brickManager.HasBoss ) {
            Debug.LogWarning("A boss is already active.");
            return;
        }

        Brick bossBrick = bricks.FirstOrDefault(b => b.isBoss || b.type == BrickType.Boss);

        if ( bossBrick == null || bossBrick.prefab == null ) {
            Debug.LogWarning("Boss brick prefab is not configured on SpawnController.");
            return;
        }



        // One boss object represents the complete 8 x 3 reserved area.
        brickManager.ClearBricksInRows(0, BrickManager.BossReservedRows);

        Vector2Int spawnPos = new(column / 2 - 1, BrickManager.BossReservedRows / 2);
        Vector3 worldPos = GetBossWorldPosition();
        GameObject brick = resolver.Instantiate(bossBrick.prefab, worldPos, Quaternion.identity);
        brick.name = bossBrick.prefab.name;
        brick.transform.SetParent(Pool.transform, true);
        brickManager.RegisterBrick(brick.GetComponent<BrickScript>(), spawnPos);
        FitBossToReservedArea(brick);
        FitBossVisualToReservedArea(brick);
    }

    Vector3 GetBossWorldPosition() {
        float topRowY = GetBrickWorldPosition(Vector2Int.zero).y;
        float bossCenterY = topRowY - squareSize * (BrickManager.BossReservedRows - 1) / 2f;
        return new Vector3(0f, bossCenterY, 0f);
    }

    void FitBossToReservedArea( GameObject boss ) {
        if ( !boss.TryGetComponent<BoxCollider2D>(out var collider) ) {
            Debug.LogWarning("Boss prefab needs a BoxCollider2D to fit the reserved grid area.");
            return;
        }

        Vector2 currentSize = collider.bounds.size;
        if ( currentSize.x <= Mathf.Epsilon || currentSize.y <= Mathf.Epsilon ) {
            Debug.LogWarning("Boss collider has no usable bounds for grid fitting.");
            return;
        }

        Vector2 targetSize = new(column * squareSize, BrickManager.BossReservedRows * squareSize);
        Vector3 scale = boss.transform.localScale;
        boss.transform.localScale = new Vector3(
            scale.x * targetSize.x / currentSize.x,
            scale.y * targetSize.y / currentSize.y,
            scale.z
        );
    }

    void FitBossVisualToReservedArea( GameObject boss ) {
        SpriteRenderer visual = boss.GetComponentsInChildren<SpriteRenderer>(true)
            .FirstOrDefault(renderer => renderer.sprite != null && renderer.gameObject.name == "Brick_Visual");

        if ( visual == null ) {
            Debug.LogWarning("Boss prefab needs a Brick_Visual SpriteRenderer to fit its artwork to the grid.");
            return;
        }

        Vector2 currentSize = visual.bounds.size;
        if ( currentSize.x <= Mathf.Epsilon || currentSize.y <= Mathf.Epsilon ) {
            Debug.LogWarning("Boss visual has no usable bounds for grid fitting.");
            return;
        }

        Vector2 targetSize = new(column * squareSize, BrickManager.BossReservedRows * squareSize);
        Vector3 scale = visual.transform.localScale;
        visual.transform.localScale = new Vector3(
            scale.x * targetSize.x / currentSize.x,
            scale.y * targetSize.y / currentSize.y,
            scale.z
        );
    }

    void OverideBrick( Vector2Int pos ) {
        BrickScript existingBrick = brickManager.GetBrickAt(pos);
        if ( existingBrick != null ) {
            Destroy(existingBrick.gameObject);
        }

        brickManager.bricks[pos.x, pos.y] = null;

    }


    /// <summary>
    /// use to spawn brick at specific position (split brick)
    /// </summary>
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

    public void SpawnSplitChildren( BrickScript origin ) {
        if ( origin == null ) {
            return;
        }

        Vector2Int pos = origin.GridPosition;
        SpawnBrickAt(new Vector2Int(pos.x - 1, pos.y));
        SpawnBrickAt(new Vector2Int(pos.x + 1, pos.y));
    }

    /// <summary>
    /// Returns the budget for the wave that is currently being spawned.
    /// The value is intentionally public so a balance simulator or debug UI can use
    /// exactly the same source of truth as runtime spawning.
    /// </summary>
    public float GetCurrentWaveBudget() {
        int waveIndex = waveScript != null ? waveScript.GetWaveIndex() : 0;
        float rawBudget = baseWaveBudget + waveIndex * budgetGrowthPerWave;
        float budget = rawBudget * GetDifficultyBudgetMultiplier();
        BossSpawnAdapter adapter = GetActiveBossSpawnAdapter();
        return adapter != null ? adapter.ModifyWaveBudget(budget) : budget;
    }

    void SpawnBricksForBudget( List<Vector2Int> positions, float budget ) {
        Shuffle(positions);

        foreach ( var position in positions ) {
            if ( brickManager.IsPositionOccupied(position) ) {
                continue;
            }

            Brick brickDefinition = GetRandomBrick(budget);
            if ( brickDefinition == null ) {
                break;
            }

            SpawnBrickDefinition(brickDefinition, position);
            budget -= brickDefinition.budgetCost;
        }
    }

    void SpawnBrickDefinition( Brick brickDefinition, Vector2Int position ) {
        Vector3 worldPosition = GetBrickWorldPosition(position);
        GameObject brick = resolver.Instantiate(brickDefinition.prefab, worldPosition, Quaternion.identity);
        brick.name = brickDefinition.prefab.name;
        brick.transform.SetParent(Pool.transform, true);
        brickManager.RegisterBrick(brick.GetComponent<BrickScript>(), position);
    }

    float GetDifficultyBudgetMultiplier() {
        return difficult switch {
            0 => easyBudgetMultiplier,
            1 => normalBudgetMultiplier,
            2 => hardBudgetMultiplier,
            _ => normalBudgetMultiplier,
        };
    }

    void Shuffle( List<Vector2Int> positions ) {
        for ( int i = positions.Count - 1; i > 0; i-- ) {
            int randomIndex = Random.Range(0, i + 1);
            Vector2Int temporary = positions[i];
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temporary;
        }
    }

    Brick GetRandomBrick( float remainingBudget ) {
        int waveIndex = waveScript.GetWaveIndex();
        var eligibleBricks = bricks.Where(b =>
            b != null &&
            b.prefab != null &&
            waveIndex >= b.minWave &&
            !b.isMiniBoss &&
            !b.isBoss &&
            b.budgetCost <= remainingBudget
        ).ToList();

        if ( eligibleBricks.Count == 0 ) {
            return null;
        }

        BossSpawnAdapter adapter = GetActiveBossSpawnAdapter();
        if ( adapter != null && adapter.TryGetReplacementBrickType(out BrickType replacementType) ) {
            Brick replacement = bricks.FirstOrDefault(b =>
                b != null &&
                b.prefab != null &&
                b.type == replacementType &&
                !b.isMiniBoss &&
                !b.isBoss &&
                b.budgetCost <= remainingBudget
            );

            if ( replacement != null ) {
                return replacement;
            }
        }

        List<float> chances = new List<float>();
        foreach ( var b in eligibleBricks ) {
            float waveBonus = (waveIndex - b.minWave) * b.growthPerWave;
            chances.Add(Mathf.Max(0f, b.baseChance + waveBonus));
        }

        float total = chances.Sum();
        if ( total <= 0f ) {
            return eligibleBricks[Random.Range(0, eligibleBricks.Count)];
        }

        // Normalize chances
        List<float> normalized = chances.Select(ch => ch / total).ToList();

        // Weighted random selection
        float rand = Random.value;
        float cumulative = 0f;
        for ( int i = 0; i < eligibleBricks.Count; i++ ) {
            cumulative += normalized[i];
            if ( rand <= cumulative ) {
                return eligibleBricks[i];
            }
        }


        // Fallback
        return eligibleBricks.Last();
    }

    BossSpawnAdapter GetActiveBossSpawnAdapter() {
        BrickScript activeBoss = brickManager.ActiveBoss;
        return activeBoss != null ? activeBoss.GetComponent<BossSpawnAdapter>() : null;
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
