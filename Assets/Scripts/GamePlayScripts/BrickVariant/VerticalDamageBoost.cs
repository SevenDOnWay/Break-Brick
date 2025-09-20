using UnityEngine;
using VContainer;
public class VerticalDamageBoost : MonoBehaviour, IBrickVariant
{
    [Inject] BrickManager brickManager;
    [Inject] SpawnController spawnController;

    private BrickScript brickScript;

    void OnEnable()
    {
        brickScript = gameObject.GetComponent<BrickScript>();
    }
    public void OnDie(BrickScript brickScript)
    {
        var pos = spawnController.GetBrickGridIndex(brickScript.transform.position);
        DealDamageVertical(pos);
    }

    public void OnEndTurn(BrickScript brickScript)
    {

    }

    public void OnHit(BrickScript brickScript)
    {

    }

    public void OnSpawn(BrickScript brickScript)
    {
    }

    private void DealDamageVertical(Vector2Int pos)
    {
        brickManager.DealDamageVertical(pos.y);
    }
}
