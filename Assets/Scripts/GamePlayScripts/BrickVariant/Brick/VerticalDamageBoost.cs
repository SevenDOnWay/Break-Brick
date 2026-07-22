using UnityEngine;
using VContainer;
public class VerticalDamageBoost : MonoBehaviour, IBrickVariant {
    IBrickGridContext brickGridContext;

    [Inject]
    void Constructor( IBrickGridContext brickGridContext ) {
        this.brickGridContext = brickGridContext;
    }

    #region IBrickVariant implementation
    public void OnHit( BrickScript brickScript ) {
        brickGridContext?.DamageColumn(brickScript, 1, DamageSource.Vertical);

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

}
