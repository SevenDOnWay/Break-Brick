using UnityEngine;
using VContainer;
public class HorizontalDamageBoost : MonoBehaviour, IBrickVariant {
    BrickManager brickManager;

    [Inject]
    void Constructor( BrickManager brickManager ) {
        this.brickManager = brickManager;
    }

    public void OnSpawn( BrickScript brickScript ) { }
    public void OnEndTurn( BrickScript brickScript ) { }

    public void OnDie( BrickScript brickScript ) {
        if( pos == null ) pos = brickManager.GetBrickGridIndex(brickScript.transform.position);

        DealDamage(brickScript, pos);
    }

    private void DealDamage( BrickScript brickScript, Vector2Int pos ) {
        brickManager.DealDamageHorizontal(pos);
    }

    public BrickType GetBrickType() {
        return BrickType.HorizontalDamageBoost;
    }
}
