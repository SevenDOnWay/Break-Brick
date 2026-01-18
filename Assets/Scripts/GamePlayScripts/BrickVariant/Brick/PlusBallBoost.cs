using UnityEngine;
using VContainer;

public class PlusBallBoost : MonoBehaviour, IBrickVariant {
    BallManager ballManager;

    [Inject]
    void Constructor( BallManager ballManager ) {
        this.ballManager = ballManager;
    }
    public void OnHit( BrickScript brickScript ) {

    }

    public void OnSpawn( BrickScript brickScript ) {

    }
    public void OnEndTurn( BrickScript brickScript ) {

    }
    public void OnDie( BrickScript brickScript ) {
        ballManager.RequestExtraBall();
    }

    public BrickType GetBrickType() {
        return BrickType.PlusBallBoost;
    }
}