using UnityEngine;
using VContainer;
public class HorizontalDamageBoost : MonoBehaviour, IBrickVariant{
    BrickManager brickManager;
    Vector2Int? pos = null;

    [Inject]
    void Constructor( BrickManager brickManager ) {
        this.brickManager = brickManager;
    }

    public void OnHit(BrickScript brickScript ) { }
    public void OnSpawn( BrickScript brickScript ) { }
    public void OnEndTurn( BrickScript brickScript ) { }

    public void OnDie( BrickScript brickScript ) {
        if( pos == null ) pos = brickManager.GetBrickGridIndex(brickScript.transform.position);
        DealDamage(brickScript, pos.Value);
        //add vfx here

    }

    private void DealDamage( BrickScript brickScript, Vector2Int pos ) {
        brickManager.DealDamageHorizontal(pos);
    }

    public BrickType GetBrickType() {
        return BrickType.HorizontalDamageBoost;
    }
}
