using UnityEngine;
using VContainer;

public class SplitBrick : MonoBehaviour, IBrickVariant {
    PlayScreen playScreen;
    BrickManager brickManager;
    SpawnController spawnController;

    Vector2Int pos;

    [Inject]
    void Constructor(
        PlayScreen playScreen,
        BrickManager brickManager,
        SpawnController spawnController
    ) {
        this.playScreen = playScreen;
        this.brickManager = brickManager;
        this.spawnController = spawnController;
    }

    public void OnDie( BrickScript brickScript ) {
        pos = brickManager.GetBrickGridIndex(brickScript.transform.position);
        Split(pos);
    }

    void Split( Vector2Int pos ) {
        //spawnController.SpawnBrickAt(new Vector2Int(pos.x - playScreen.squareSize, pos.y));
        //spawnController.SpawnBrickAt(new Vector2Int(pos.x + playScreen.squareSize, pos.y));
        spawnController.SpawnBrickAt(new Vector2Int(pos.x - 1, pos.y));
        spawnController.SpawnBrickAt(new Vector2Int(pos.x + 1, pos.y));
    }

    public BrickType GetBrickType( ) {
        return BrickType.Split;
    }

    public void OnSpawn( BrickScript brickScript ) {}
    public void OnHit( BrickScript brickScript ) {}
    public void OnEndTurn( BrickScript brickScript ) {}
}
