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
        ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.Reinforce, brickScript.transform.position, radius: brickScript.SquareSize));
    }

    public void OnEndTurn( BrickScript brickScript ) {
        turnsUntilRally--;
        if ( turnsUntilRally > 0 ) return;

        var healed = brickManager?.RequestHeal(brickScript, brickScript.GridPosition, rallyRadius, rallyHealAmount);
        if ( healed != null && healed.Count > 0 ) {
            var targets = new System.Collections.Generic.List<Vector3>();
            foreach (var target in healed) targets.Add(target.transform.position);
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.HealPulse, brickScript.transform.position, radius: brickScript.SquareSize * 1.25f, intensity: 1.3f, targetPositions: targets));
        }
        turnsUntilRally = Mathf.Max(1, rallyCooldownTurns);
    }

    public void OnHit( BrickScript brickScript ) { }
    public void OnDie( BrickScript brickScript ) { }
}
