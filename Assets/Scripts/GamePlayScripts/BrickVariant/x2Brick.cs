using UnityEngine;

public class x2Brick : MonoBehaviour, IBrickVariant {
    public void OnSpawn( BrickScript brickScript ) {
        brickScript.health *= 2;
    }

    public BrickType GetBrickType() {
        return BrickType.x2health;
    }

    public void OnDie( BrickScript brickScript ) { }

    public void OnEndTurn( BrickScript brickScript ) { }

    public void OnHit( BrickScript brickScript ) { }


}
