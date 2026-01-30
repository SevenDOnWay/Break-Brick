using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using static BrickData;
using static UnityEditor.PlayerSettings;

public class BrickManager : MonoBehaviour {

    RunDataManager runDataManager;
    PlayScreen playScreen;
    WaveScript waveScript;

    const int row = 10;
    const int column = 8;

    [SerializeField] AnimationCurve curve; //handle health of the brick


    // List<BrickScript> bricks = new List<BrickScript>();
    public BrickScript[,] bricks = new BrickScript[column, row];
    // public IReadOnlyList<BrickScript> Bricks => bricks;


    Queue<DamageRequest> damageQueue = new();
    HashSet<BrickScript> damagedThisTick = new();

    private float squareSize;

    [Inject]
    public void Constructor(
        RunDataManager runDataManager,
        PlayScreen playScreen,
        WaveScript waveScript
     ) {
        this.runDataManager = runDataManager;
        this.playScreen = playScreen;
        this.waveScript = waveScript;
    }


    public void Start() {
        squareSize = playScreen.squareSize;
    }

    public void StartGame() {

    }

    public void Update() {
        damagedThisTick.Clear();

        while ( damageQueue.Count > 0 ) {
            var request = damageQueue.Dequeue();

            if ( request.target == null || request.target.IsDead ) continue;
            if ( damagedThisTick.Contains(request.target) ) continue;

            request.target.ApplyDamageInternal(request);
            damagedThisTick.Add(request.target);
        }
    }

    public void RegisterBrick( BrickScript brick, Vector2Int pos, int? savedHealth = null ) {

        // var brick = gameobject.GetComponent<BrickScript>();
        int health;

        if ( savedHealth.HasValue ) {
            health = savedHealth.Value;
        }
        else {
            float value = curve.Evaluate(waveScript.GetWaveIndex());
            health = Mathf.CeilToInt(value);
        }

        brick.Init(health);
        brick.UpdateGridPosition(pos);

        bricks[pos.x, pos.y] = brick;

        brick.OnHit += HandleBrickHit;
        brick.OnDestroyed += HandleBrickDestroyed;
    }

    void HandleBrickHit( BrickScript brick, DamageSource source, int damage = 1) {
        RequestDamage(new DamageRequest(brick, damage, source, null));
    }

    private void HandleBrickDestroyed( Vector2Int pos ) {
        if ( pos.x < 0 || pos.x >= column || pos.y < 0 || pos.y >= row ) {
            Debug.LogWarning($"Attempted to remove brick at invalid position: {pos}");
            return;
        }

        bricks[pos.x, pos.y] = null;
    }


    public void MoveBrick() {
        for ( int y = row - 1; y >= 0; y-- ) {
            for ( int x = column - 1; x >= 0; x-- ) {

                var brick = bricks[x, y];

                if ( brick == null ) continue; // Skip if the brick is null
                //Debug.Log($"{x}, {y}");
                brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);
                brick.UpdateGridPosition(new Vector2Int(x, y + 1));

                // TODO Game Over
                if ( y == row - 1 ) {
                    Debug.Log("Game Over!");

                    HandleBrickDestroyed(new Vector2Int(x, y)); //delete brick from grid for now
                    continue;
                }

                bricks[x, y + 1] = brick;
                bricks[x, y] = null;
            }
        }
    }

    public void RequestDamage(
    BrickScript target,
    int damage,
    DamageSource source,
    BrickScript origin = null
    ) {
        if ( target == null ) return;

        damageQueue.Enqueue(new DamageRequest(target, damage, source, null));
    }

    public void RequestDamage( DamageRequest damageRequest ) {
        if ( damageRequest.target == null ) return;

        damageQueue.Enqueue(damageRequest);
    }

    public void RequestHorizontalDamage( BrickScript origin, Vector2Int pos, int damage = 1 ) {
        int rowIndex = pos.y;

        for ( int x = 0; x < column; x++ ) {
            var brick = bricks[x, rowIndex];
            if ( brick == null || brick == origin ) continue;

            RequestDamage(brick, damage, DamageSource.Horizontal, origin);
        }
    }

    public void RequestVerticalDamage( BrickScript origin, Vector2Int pos, int damage = 1 ) {
        int colIndex = pos.x;

        for ( int y = 0; y < row; y++ ) {
            var brick = bricks[colIndex, y];
            if ( brick == null || brick == origin ) continue;

            RequestDamage(brick, damage, DamageSource.Vertical, origin);
        }
    }


    //public void DealDamageHorizontal( Vector2Int pos , int dame = 1) {
    //    int colIndex = pos.x;
    //    int rowIndex = pos.y;

    //    for ( int x = 0; x < column; x++ ) {
    //        if ( x == colIndex ) continue; // skip the original brick

    //        var brick = bricks[x, rowIndex];
    //        if ( brick == null ) continue;

    //        brick.TakeDamage(dame);
    //    }
    //}
    //public void DealDamageVertical( Vector2Int pos ) {
    //    int colIndex = pos.x;
    //    int rowIndex = pos.y;

    //    for ( int y = 0; y < row; y++ ) {
    //        if ( y == rowIndex ) continue; // skip the original brick

    //        var brick = bricks[colIndex, y];
    //        if ( brick == null ) continue;

    //        brick.TakeDamage(1);
    //    }
    //}

    public void SaveBrick() {
        List<BrickData> newBricks = new List<BrickData>();

        for ( int col = 0; col < column; col++ ) {
            for ( int r = 0; r < row; r++ ) {
                var brick = bricks[col, r];
                if ( brick == null ) continue; // skip empty grid slots

                try {
                    int hp = brick.health;

                    BrickType type = BrickType.Normal;
                    if ( brick.TryGetComponent<IBrickVariant>(out var brickType) ) {
                        type = brickType.GetBrickType();
                    }

                    newBricks.Add(new BrickData(col, r, hp, type));
                }
                catch ( System.Exception e ) {
                    Debug.LogWarning($"Error saving brick at [{col},{row}]: {e.Message}");
                    continue;
                }
            }
        }

        runDataManager.runData.OverwriteBricksData(newBricks);
    }

    public Vector2Int GetBrickGridIndex( Vector3 worldPos ) {
        float startX = (column - 1) * playScreen.squareSize / 2f;
        float startY = (row - 1) * playScreen.squareSize / 2f;

        int x = Mathf.RoundToInt((worldPos.x + startX) / playScreen.squareSize);
        int y = Mathf.RoundToInt((startY - worldPos.y) / playScreen.squareSize);

        //Debug.Log($"{x} and {y}");

        return new Vector2Int(x, y);
    }

    public bool IsPositionOccupied( Vector2Int pos ) {
        if ( pos.x < 0 || pos.x >= column || pos.y < 0 || pos.y >= row ) {
            return true; // Out of bounds is considered occupied
        }
        return bricks[pos.x, pos.y] != null;
    }

    //public Vector2Int? FindBrickPosition( BrickScript target ) {
    //    for ( int x = 0; x < column; x++ ) {
    //        for ( int y = 0; y < row; y++ ) {
    //            if ( bricks[x, y] == target ) {
    //                return new Vector2Int(x, y); // Found
    //            }
    //        }
    //    }
    //    return null; // Not Found
    //}
}


public readonly struct DamageRequest {
    public readonly BrickScript target;
    public readonly int damage;
    public readonly DamageSource source;
    public readonly BrickScript origin;

    public DamageRequest(
        BrickScript target,
        int damage,
        DamageSource source,
        BrickScript origin = null
    ) {
        this.target = target;
        this.damage = damage;
        this.source = source;
        this.origin = origin;
    }
}