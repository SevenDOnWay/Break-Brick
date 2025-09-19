using UnityEngine;
using VContainer;

public class SplitBrick : MonoBehaviour, IBrickVariant
{
    [Inject] PlayScreen playScreen;
    [Inject] SpawnController spawnController;
    BrickScript brickScript;

    Vector2Int pos;

    void OnEnable()
    {
        brickScript = gameObject.GetComponent<BrickScript>();
    }

    public void OnDie(BrickScript brickScript)
    {
        pos = spawnController.GetBrickGridIndex(brickScript.transform.position);
        Split(pos);
    }

    void Split(Vector2Int pos)
    {
        // spawnController.SpawnBrickAt(new Vector2(pos.x - playScreen.squareSize, pos.y));
        // spawnController.SpawnBrickAt(new Vector2(pos.x + playScreen.squareSize, pos.y));
        spawnController.SpawnBrickAt(new Vector2Int(pos.x - 1, pos.y));
        spawnController.SpawnBrickAt(new Vector2Int(pos.x + 1, pos.y));
    }

    public void OnSpawn(BrickScript brickScript) { }
    public void OnHit(BrickScript brickScript) { }
    public void OnEndTurn(BrickScript brickScript) { }
}
