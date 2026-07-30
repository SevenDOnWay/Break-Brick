using UnityEngine;
using VContainer;

/// <summary>
/// Brick variant that periodically heals adjacent bricks in its grid neighbourhood.
/// Every <see cref="healCooldownTurns"/> turns it searches within <see cref="healRadius"/>
/// (Manhattan + diagonals) and restores <see cref="healAmount"/> HP to each living neighbour.
/// </summary>
/// <remarks>
/// Turn counting is driven via <see cref="OnEndTurn"/> when bricks advance. The
/// Grid changes are owned by <see cref="BrickManager"/>.
/// </remarks>
public class HealingBrick : MonoBehaviour, IBrickVariant {
    [SerializeField] int healCooldownTurns = 3;
    [SerializeField] int healAmount = 1;
    [SerializeField] int healRadius = 1;

    BrickManager brickManager;
    int turnCounter;

    // ── VContainer ─────────────────────────────────────────────────────────────

    [Inject]
    void Constructor( BrickManager brickManager ) {
        this.brickManager = brickManager;
    }

    // ── IBrickVariant ──────────────────────────────────────────────────────────

    public BrickType GetBrickType() => BrickType.Healing;

    public void OnSpawn( BrickScript brickScript ) {
        turnCounter = healCooldownTurns;
    }

    public void OnEndTurn( BrickScript brickScript ) {
        turnCounter--;
        if ( turnCounter > 0 ) return;

        var healed = brickManager?.RequestHeal(brickScript, brickScript.GridPosition, healRadius, healAmount);
        if ( healed != null && healed.Count > 0 ) {
            var targets = new System.Collections.Generic.List<Vector3>();
            foreach (var target in healed) targets.Add(target.transform.position);
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.HealPulse, brickScript.transform.position, radius: brickScript.SquareSize, targetPositions: targets));
        }
        turnCounter = healCooldownTurns;
    }

    public void OnHit( BrickScript brickScript ) { }
    public void OnDie( BrickScript brickScript ) { }
}
