using UnityEngine;
using VContainer;
public class HorizontalDamageBoost : MonoBehaviour, IBrickVariant{
    BrickManager brickManager;

    [Inject]
    void Constructor( BrickManager brickManager ) {
        this.brickManager = brickManager;
    }

    #region IBrickVariant implementation
    public void OnHit(BrickScript brickScript ) {
        brickManager?.RequestHorizontalDamage(brickScript, brickScript.GridPosition, 1);

        //add vfx here
        //TODO: FIX name
        Vector2 tempPos = transform.position;
        VFXEvent.RaiseVFXCommand(new BeamVFXCommand(tempPos, Vector2.left));
    }
    public void OnSpawn( BrickScript brickScript ) { }
    public void OnEndTurn( BrickScript brickScript ) { }

    public void OnDie( BrickScript brickScript ) {}

    public BrickType GetBrickType() {
        return BrickType.HorizontalDamageBoost;
    }
    #endregion

}
