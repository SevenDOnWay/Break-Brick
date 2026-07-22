using UnityEngine;
using VContainer;

/// <summary>
/// Brick variant that detonates on death, dealing area damage to all bricks
/// within a configurable Chebyshev radius (i.e. 8-connected square at radius 1).
/// </summary>
/// <remarks>
/// Damage is delegated through <see cref="BrickManager"/> so the grid owns
/// queueing and chain-depth limits.
/// </remarks>
public class TntBrick : MonoBehaviour, IBrickVariant {
    [SerializeField] int explosionRadius = 1;
    [SerializeField] int explosionDamage = 3;

    BrickManager brickManager;

    // ── VContainer ─────────────────────────────────────────────────────────────

    [Inject]
    void Constructor( BrickManager brickManager ) {
        this.brickManager = brickManager;
    }

    // ── IBrickVariant ──────────────────────────────────────────────────────────

    public BrickType GetBrickType() => BrickType.TNT;

    public void OnDie( BrickScript brickScript ) {
        brickManager?.RequestRadialDamage(brickScript, brickScript.GridPosition, explosionRadius, explosionDamage, DamageSource.TNT);
    }

    public void OnSpawn( BrickScript brickScript ) { }
    public void OnHit( BrickScript brickScript ) { }
    public void OnEndTurn( BrickScript brickScript ) { }
}
