using UnityEngine;
using VContainer;
public class HorizontalDamageBoost : MonoBehaviour, IBrickVariant{
    BrickManager brickManager;
    StatManager statManager;
    Vector2Int? pos = null;

    [Inject]
    void Constructor( BrickManager brickManager, StatManager statManager) {
        this.brickManager = brickManager;
        this.statManager = statManager;
    }

    public void OnHit(BrickScript brickScript ) {
        if ( pos == null ) pos = brickManager.GetBrickGridIndex(brickScript.transform.position);
        DealDamage(brickScript, pos.Value);
        //add vfx here

        //TODO: Not Useded currently
        var radius = statManager.GetStat(UpgradeType.ExplosionRadius);

        //TODO: FIX name
        Vector2 tempPos = transform.position;
        VFXEvent.RaiseVFXCommand(new BeamVFXCommand(tempPos));
    }
    public void OnSpawn( BrickScript brickScript ) { }
    public void OnEndTurn( BrickScript brickScript ) { }

    public void OnDie( BrickScript brickScript ) {
    }

    private void DealDamage( BrickScript brickScript, Vector2Int pos ) {
        brickManager.DealDamageHorizontal(pos);
    }

    public BrickType GetBrickType() {
        return BrickType.HorizontalDamageBoost;
    }
}
