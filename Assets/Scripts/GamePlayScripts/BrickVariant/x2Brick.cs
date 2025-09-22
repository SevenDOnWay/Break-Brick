using UnityEngine;

public class x2Brick : MonoBehaviour, IBrickVariant {
    public void OnDie( BrickScript brickScript ) { }

    public void OnEndTurn( BrickScript brickScript ) { }

    public void OnHit( BrickScript brickScript ) { }

    public void OnSpawn( BrickScript brickScript ) {
        brickScript.health *= 2;
        brickScript.UpdateHealthText();
    }

}
