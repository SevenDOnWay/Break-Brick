using UnityEngine;

public class x2Brick : MonoBehaviour, IBrickVariant {

    public void OnDie( BrickScript brickScript ) { }

    public void OnEndTurn( BrickScript brickScript ) { }

    public void OnHit( BrickScript brickScript ) { }

    public void OnSpawn( BrickScript brickScript ) {
        if ( brickScript.IsRestoringSavedHealth ) return;

        brickScript.health *= 2;
        brickScript.UpdateHealthText();
        ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.Reinforce, brickScript.transform.position, radius: brickScript.SquareSize));
    }

    public BrickType GetBrickType() {
        return BrickType.x2health;
    }

}
