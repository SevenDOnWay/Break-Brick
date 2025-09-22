using UnityEngine;
using VContainer;
public class VerticalDamageBoost : MonoBehaviour, IBrickVariant {
    [Inject] BrickManager brickManager;

    public void OnEndTurn( BrickScript brickScript ) { }
    public void OnHit( BrickScript brickScript ) { }
    public void OnSpawn( BrickScript brickScript ) { }


    public void OnDie( BrickScript brickScript ) {
        var pos = brickManager.GetBrickGridIndex(brickScript.transform.position);
        DealDamage(pos);
    }

    private void DealDamage( Vector2Int pos ) {
        brickManager.DealDamageVertical(pos);
    }
}
