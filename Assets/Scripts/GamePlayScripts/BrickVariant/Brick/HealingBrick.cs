using UnityEngine;
using VContainer;

/// <summary>
/// Brick variant that periodically heals adjacent bricks in its grid neighbourhood.
/// Every <see cref="healCooldownTurns"/> turns it searches within <see cref="healRadius"/>
/// (Manhattan + diagonals) and restores <see cref="healAmount"/> HP to each living neighbour.
/// </summary>
/// <remarks>
/// Turn counting is driven via <see cref="OnEndTurn"/> which is called by BrickManager
/// each time bricks advance. Injects only <see cref="BrickManager"/> — no ball knowledge.
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

        brickManager.RequestHeal(brickScript, brickScript.GridPosition, healRadius, healAmount);
        turnCounter = healCooldownTurns;
    }

    public void OnHit( BrickScript brickScript ) { }
    public void OnDie( BrickScript brickScript ) { }
}
