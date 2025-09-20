using UnityEngine;
using VContainer;
public class HorizontalDamageBoost : MonoBehaviour, IBrickVariant
{
    [Inject] BrickManager brickManager;
    [Inject] SpawnController spawnController;
    private BrickScript brickScript;

    void OnEnable()
    {
        brickScript = gameObject.GetComponent<BrickScript>();
    }

    public void OnHit(BrickScript brickScript)
    {
    }

    public void OnSpawn(BrickScript brickScript)
    {
    }
    public void OnEndTurn(BrickScript brickScript)
    {

    }
    public void OnDie(BrickScript brickScript)
    {
        var pos = spawnController.GetBrickGridIndex(brickScript.transform.position);
        DealDamageHorizontal(pos);

    }

    private void DealDamageHorizontal(Vector2Int pos)
    {
        brickManager.DealDamageHorizontal(pos.y);
    }
}
