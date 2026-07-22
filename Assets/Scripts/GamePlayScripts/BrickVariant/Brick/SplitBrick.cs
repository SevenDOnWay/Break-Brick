using UnityEngine;
using VContainer;

public class SplitBrick : MonoBehaviour, IBrickVariant {
    IBrickSpawnContext brickSpawnContext;

    [Inject]
    void Constructor( IBrickSpawnContext brickSpawnContext ) {
        this.brickSpawnContext = brickSpawnContext;
    }

    public void OnDie( BrickScript brickScript ) {
        brickSpawnContext?.SpawnSplitChildren(brickScript);
    }

    public BrickType GetBrickType( ) {
        return BrickType.Split;
    }

    public void OnSpawn( BrickScript brickScript ) {}
    public void OnHit( BrickScript brickScript ) {}
    public void OnEndTurn( BrickScript brickScript ) {}
}
