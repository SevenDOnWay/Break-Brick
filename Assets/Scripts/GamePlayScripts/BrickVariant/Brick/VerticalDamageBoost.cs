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
        brickManager?.RequestVerticalDamage(brickScript, brickScript.GridPosition, 1);

        Vector2 tempPos = transform.position;
        VFXEvent.RaiseVFXCommand(new BeamVFXCommand(tempPos, Vector2.up));
    }
    public void OnEndTurn( BrickScript brickScript ) { }
    public void OnSpawn( BrickScript brickScript ) { }


    public void OnDie( BrickScript brickScript ) { }

    public BrickType GetBrickType() {
        return BrickType.VerticalDamageBoost;
    }
    #endregion

}
