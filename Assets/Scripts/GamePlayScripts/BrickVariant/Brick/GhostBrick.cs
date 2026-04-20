using UnityEngine;

/// <summary>
/// Brick variant that alternates between Solid and Ghost phases each
/// <see cref="toggleIntervalTurns"/> turns.
/// While in Ghost phase its <see cref="Collider2D"/> is disabled (balls pass through)
/// and the sprite alpha drops to <see cref="ghostAlpha"/>.
/// </summary>
/// <remarks>
/// Uses GetComponent once at <see cref="OnSpawn"/> — acceptable for a one-time init call
/// that is not on a hot path. No VContainer injection needed; all dependencies live on the
/// same GameObject.
/// </remarks>
public class GhostBrick : MonoBehaviour, IBrickVariant {
    [SerializeField] int toggleIntervalTurns = 2;
    [SerializeField] float ghostAlpha = 0.25f;

    Collider2D brickCollider;
    SpriteRenderer spriteRenderer;
    bool isSolid = true;
    int turnCounter;

    // ── IBrickVariant ──────────────────────────────────────────────────────────

    public BrickType GetBrickType() => BrickType.Ghost;

    public void OnSpawn( BrickScript brickScript ) {
        brickCollider = brickScript.GetComponent<Collider2D>();
        spriteRenderer = brickScript.GetComponentInChildren<SpriteRenderer>();
        turnCounter = toggleIntervalTurns;
        ApplyPhase();
    }

    public void OnEndTurn( BrickScript brickScript ) {
        turnCounter--;
        if ( turnCounter > 0 ) return;

        isSolid = !isSolid;
        ApplyPhase();
        turnCounter = toggleIntervalTurns;
    }

    public void OnHit( BrickScript brickScript ) { }
    public void OnDie( BrickScript brickScript ) { }

    // ── Helpers ────────────────────────────────────────────────────────────────

    void ApplyPhase() {
        if ( brickCollider != null ) {
            brickCollider.enabled = isSolid;
        }

        if ( spriteRenderer != null ) {
            Color color = spriteRenderer.color;
            color.a = isSolid ? 1f : ghostAlpha;
            spriteRenderer.color = color;
        }
    }
}
