using UnityEngine;

public interface IBrickVariant {
    void OnSpawn( BrickScript brickScript );
    void OnHit( BrickScript brickScript );
    void OnEndTurn( BrickScript brickScript );
    void OnDie( BrickScript brickScript );
}
