using UnityEngine;
using VContainer;
public class HorizontalDamageBoost : MonoBehaviour, IBrickVariant{
    IBrickGridContext brickGridContext;

    [Inject]
    void Constructor( IBrickGridContext brickGridContext) {
        this.brickGridContext = brickGridContext;
    }

    #region IBrickVariant implementation
    public void OnHit(BrickScript brickScript ) {
        brickGridContext?.DamageRow(brickScript, 1, DamageSource.Horizontal);

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

}
