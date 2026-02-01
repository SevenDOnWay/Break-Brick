using UnityEngine;
using VContainer;
public class HorizontalDamageBoost : MonoBehaviour, IBrickVariant{
    BrickManager brickManager;
    StatManager statManager;

    [Inject]
    void Constructor( BrickManager brickManager, StatManager statManager) {
        this.brickManager = brickManager;
        this.statManager = statManager;
    }

    #region IBrickVariant implementation
    public void OnHit(BrickScript brickScript ) {
        var gridPos = brickManager.GetBrickGridIndex(brickScript.transform.position);
        DealDamage(brickScript, gridPos);

        //add vfx here
        //TODO: FIX name
        Vector2 tempPos = transform.position;
        VFXEvent.RaiseVFXCommand(new HorizontalBeamVFXCommand(tempPos));
    }
    public void OnSpawn( BrickScript brickScript ) { }
    public void OnEndTurn( BrickScript brickScript ) { }

    public void OnDie( BrickScript brickScript ) {}

    public BrickType GetBrickType() {
        return BrickType.HorizontalDamageBoost;
    }
    #endregion

    private void DealDamage( BrickScript brickScript, Vector2Int pos ) {
        brickManager.RequestHorizontalDamage(brickScript, pos);
    }

    
}
