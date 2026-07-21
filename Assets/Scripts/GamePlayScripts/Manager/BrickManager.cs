using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BrickManager : MonoBehaviour {

    RunDataManager runDataManager;
    PlayScreen playScreen;
    WaveScript waveScript;
    StatManager statManager;

    const int row = 10;
    const int column = 8;

    [SerializeField] AnimationCurve curve; //handle health of the brick


    // List<BrickScript> bricks = new List<BrickScript>();
    public BrickScript[,] bricks = new BrickScript[column, row];
    // public IReadOnlyList<BrickScript> Bricks => bricks;

    public event Action GameOverEvent;

    Queue<DamageRequest> damageQueue = new();
    HashSet<BrickScript> damagedThisTick = new();

    private float squareSize;

    [Inject]
    public void Constructor(
        RunDataManager runDataManager,
        PlayScreen playScreen,
        WaveScript waveScript,
        StatManager statManager
     ) {
        this.runDataManager = runDataManager;
        this.playScreen = playScreen;
        this.waveScript = waveScript;
        this.statManager = statManager;
    }


    public void Start() {
        squareSize = playScreen.squareSize;
    }

    public void StartGame() {

    }

    const int MaxChainDepth = 3;

    public void Update() {
        damagedThisTick.Clear();

        while ( damageQueue.Count > 0 ) {
            var request = damageQueue.Dequeue();

            if ( request.depth >= MaxChainDepth ) continue;
            if ( request.target == null || request.target.IsDead ) continue;
            if ( damagedThisTick.Contains(request.target) ) continue;

            var target = request.target;
            request.target.ApplyDamageInternal(request);
            damagedThisTick.Add(target);

            // Shockwave check: if the brick died from this damage, try shockwave
            if ( target.IsDead ) {
                ShockwaveProcess.TryShockwave(statManager, this, target.GridPosition, request.depth);
            }
        }
    }

    public void HandleAllBallDone() {
        MoveBrick();
    }

    public void RegisterBrick( BrickScript brick, Vector2Int pos, int? savedHealth = null ) {

        // var brick = gameobject.GetComponent<BrickScript>();
        int health;

        if ( savedHealth.HasValue ) {
            health = savedHealth.Value;
        } else {
            float value = curve.Evaluate(waveScript.GetWaveIndex());
            health = Mathf.CeilToInt(value);
        }

        brick.Init(health, squareSize, savedHealth.HasValue);
        brick.UpdateGridPosition(pos);

        bricks[pos.x, pos.y] = brick;

        brick.OnHit += HandleBrickHit;
        brick.OnDestroyed += HandleBrickDestroyed;
    }

    void HandleBrickHit( BrickScript brick, DamageSource source, int damage = 1, Vector2 hitNormal = default ) {
        RequestDamage(new DamageRequest(brick, damage, source, null, 0, hitNormal));
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

                if ( brick == null ) continue;


                TickBrickEffects(brick);

                if ( brick.HasActiveEffect(EffectType.Freeze) ) {
                    continue;
                }

                // TODO Game Over
                if ( y == row - 1 ) {
                    //TODO: add vfx in here before invoking game over event

                    GameOverEvent.Invoke();
                }

                brick.transform.position = new Vector3(brick.transform.position.x, brick.transform.position.y - playScreen.squareSize);
                brick.UpdateGridPosition(new Vector2Int(x, y + 1));

                bricks[x, y + 1] = brick;
                bricks[x, y] = null;
            }
        }
    }

    private void TickBrickEffects( BrickScript brick ) {
        brick.TickEffects();
        brick.CallVariantEndTurn();
    }

    public void RequestDamage(
    BrickScript target,
    int damage,
    DamageSource source,
    BrickScript origin = null,
    int depth = 0
    ) {
        if ( target == null ) return;

        damageQueue.Enqueue(new DamageRequest(target, damage, source, origin, depth));
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

    public void RequestRadialDamage(
        BrickScript origin,
        Vector2Int pos,
        int radius,
        int damage,
        DamageSource source
    ) {
        for ( int dx = -radius; dx <= radius; dx++ ) {
            for ( int dy = -radius; dy <= radius; dy++ ) {
                if ( dx == 0 && dy == 0 ) continue;
                var neighbor = GetBrickAt(new Vector2Int(pos.x + dx, pos.y + dy));
                if ( neighbor == null || neighbor == origin ) continue;
                RequestDamage(neighbor, damage, source, origin);
            }
        }
    }

    /// <summary>
    /// Restores <paramref name="healAmount"/> HP to every living brick within
    /// <paramref name="radius"/> cells (Chebyshev distance) of <paramref name="pos"/>,
    /// excluding <paramref name="origin"/> itself.
    /// </summary>
    public void RequestHeal( BrickScript origin, Vector2Int pos, int radius, int healAmount ) {
        for ( int dx = -radius; dx <= radius; dx++ ) {
            for ( int dy = -radius; dy <= radius; dy++ ) {
                if ( dx == 0 && dy == 0 ) continue;

                BrickScript neighbour = GetBrickAt(new Vector2Int(pos.x + dx, pos.y + dy));
                if ( neighbour == null || neighbour == origin || neighbour.IsDead ) continue;

                neighbour.health += healAmount;
                neighbour.UpdateHealthText();
            }
        }
    }

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
                } catch ( System.Exception e ) {
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

    public BrickScript GetBrickAt( Vector2Int pos ) {
        if ( pos.x < 0 || pos.x >= column || pos.y < 0 || pos.y >= row ) {
            return null;
        }
        return bricks[pos.x, pos.y];
    }
}


public readonly struct DamageRequest {
    public readonly BrickScript target;
    public readonly int damage;
    public readonly DamageSource source;
    public readonly BrickScript origin;
    public readonly int depth;
    public readonly Vector2 hitNormal;

    public DamageRequest(
        BrickScript target,
        int damage,
        DamageSource source,
        BrickScript origin = null,
        int depth = 0,
        Vector2 hitNormal = default
    ) {
        this.target = target;
        this.damage = damage;
        this.source = source;
        this.origin = origin;
        this.depth = depth;
        this.hitNormal = hitNormal;
    }
}
