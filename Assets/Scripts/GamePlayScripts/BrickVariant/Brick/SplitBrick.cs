using UnityEngine;
using VContainer;

public class SplitBrick : MonoBehaviour, IBrickVariant {
    SpawnController spawnController;

    [Inject]
    void Constructor( SpawnController spawnController ) {
        this.spawnController = spawnController;
    }

    public void OnDie( BrickScript brickScript ) {
        spawnController?.SpawnSplitChildren(brickScript);
    }

    public BrickType GetBrickType( ) {
        return BrickType.Split;
    }

    public void OnSpawn( BrickScript brickScript ) {}
    public void OnHit( BrickScript brickScript ) {}
    public void OnEndTurn( BrickScript brickScript ) {}
}
