using NUnit.Framework.Constraints;
using UnityEngine;
using VContainer;
public class VerticalDamageBoost : MonoBehaviour, IBrickVariant {
    BrickManager brickManager;

    [Inject]
    void Constructor( BrickManager brickManager ) {
        this.brickManager = brickManager;
    }

    #region IBrickVariant implementation
    public void OnHit( BrickScript brickScript ) {
        var pos = brickManager.GetBrickGridIndex(brickScript.transform.position);
        DealDamage(brickScript, pos);

        Vector2 tempPos = transform.position;
        VFXEvent.RaiseVFXCommand(new VerticalBeamVFXCommand(tempPos));
    }
    public void OnEndTurn( BrickScript brickScript ) { }
    public void OnSpawn( BrickScript brickScript ) { }


    public void OnDie( BrickScript brickScript ) { }

    public BrickType GetBrickType() {
        return BrickType.VerticalDamageBoost;
    }
    #endregion

    private void DealDamage(BrickScript brickScript ,Vector2Int pos ) {
        brickManager.RequestVerticalDamage(brickScript, pos);
    }

    
}
