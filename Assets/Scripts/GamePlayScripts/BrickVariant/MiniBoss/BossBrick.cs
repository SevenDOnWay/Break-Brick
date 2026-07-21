using UnityEngine;
using VContainer;

/// <summary>
/// Boss brick variant. The boss enters with amplified HP and periodically restores
/// nearby bricks so the player has a durable priority target.
/// </summary>
public class BossBrick : MonoBehaviour, IBrickVariant {
    [SerializeField] int healthMultiplier = 5;
    [SerializeField] int rallyCooldownTurns = 2;
    [SerializeField] int rallyHealAmount = 2;
    [SerializeField] int rallyRadius = 1;

    BrickManager brickManager;
    int turnsUntilRally;

    [Inject]
    void Constructor( BrickManager brickManager ) {
        this.brickManager = brickManager;
    }

    public BrickType GetBrickType() => BrickType.Boss;

    public void OnSpawn( BrickScript brickScript ) {
        if ( !brickScript.IsRestoringSavedHealth ) {
            brickScript.health *= Mathf.Max(1, healthMultiplier);
            brickScript.UpdateHealthText();
        }

        turnsUntilRally = Mathf.Max(1, rallyCooldownTurns);
    }

    public void OnEndTurn( BrickScript brickScript ) {
        turnsUntilRally--;
        if ( turnsUntilRally > 0 ) return;

        brickManager.RequestHeal(brickScript, brickScript.GridPosition, rallyRadius, rallyHealAmount);
        turnsUntilRally = Mathf.Max(1, rallyCooldownTurns);
    }

    public void OnHit( BrickScript brickScript ) { }
    public void OnDie( BrickScript brickScript ) { }
}
