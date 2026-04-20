using UnityEngine;

/// <summary>
/// Brick variant that ignores damage from one specific face.
/// The protected face is defined by <see cref="shieldDirection"/>.
/// Uses a dot-product side-check: if the incoming contact normal points toward the
/// shield face (dot &gt; 0), the hit is silently cancelled. All other directions
/// bypass the shield and deal damage normally.
/// </summary>
/// <remarks>
/// Decoupled from BallScript — reacts only to data inside <see cref="DamageRequest"/>.
/// The contact normal is supplied by BallScript via <see cref="BrickScript.NotifyHit"/>.
/// </remarks>
public class ShieldedBrick : MonoBehaviour, IBrickVariant, IDamageBlocker {
    [SerializeField] Vector2 shieldDirection = Vector2.up;
    [SerializeField] GameObject shieldVisual;

    // ── IBrickVariant ──────────────────────────────────────────────────────────

    public BrickType GetBrickType() => BrickType.Shielded;

    public void OnSpawn( BrickScript brickScript ) {
        float squareSize = brickScript.SquareSize;
        float offset = squareSize / 2f;

        if ( shieldDirection == Vector2.up ) {
            shieldVisual.transform.localPosition = new Vector3(0, offset, 0);
            shieldVisual.transform.localRotation = Quaternion.Euler(0, 0, 0);
        } else if ( shieldDirection == Vector2.down ) {
            shieldVisual.transform.localPosition = new Vector3(0, -offset, 0);
            shieldVisual.transform.localRotation = Quaternion.Euler(0, 0, 180);
        } else if ( shieldDirection == Vector2.left ) {
            shieldVisual.transform.localPosition = new Vector3(-offset, 0, 0);
            shieldVisual.transform.localRotation = Quaternion.Euler(0, 0, 90);
        } else if ( shieldDirection == Vector2.right ) {
            shieldVisual.transform.localPosition = new Vector3(offset, 0, 0);
            shieldVisual.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }

        var sr = shieldVisual.GetComponent<SpriteRenderer>();
        if ( sr != null ) {
            float spriteWidth = sr.sprite.bounds.size.x;
            float scaleFactor = squareSize / spriteWidth;
            shieldVisual.transform.localScale = new Vector3(scaleFactor, scaleFactor * -0.5f, 1f);
        }
    }

    public void OnHit( BrickScript brickScript ) { }
    public void OnEndTurn( BrickScript brickScript ) { }
    public void OnDie( BrickScript brickScript ) { }

    // ── IDamageBlocker ─────────────────────────────────────────────────────────

    /// <summary>
    /// Blocks the hit when the contact normal points toward the shield face.
    /// A positive dot product means the ball arrived from the shield side.
    /// </summary>
    public bool TryBlock( DamageRequest req ) {
        if ( req.hitNormal == Vector2.zero ) return false;
        return Vector2.Dot(req.hitNormal, shieldDirection) > 0f;
    }
}
