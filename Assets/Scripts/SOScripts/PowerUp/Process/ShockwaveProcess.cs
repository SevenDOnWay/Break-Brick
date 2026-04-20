using UnityEngine;

public class ShockwaveProcess : Process {

    public override ProcessType GetProssType() => ProcessType.Shockwave;

    /// <summary>
    /// Shockwave does not trigger on ball hit.
    /// It triggers on brick death via BrickScript.OnDeath flow.
    /// This OnHit returns 0 by design.
    /// </summary>
    public override int OnHit( StatManager statManager, BrickScript brick, int baseDamage ) {
        return 0;
    }

    /// <summary>
    /// Called from BrickScript.OnDeath to check if shockwave should trigger.
    /// Queries the 4 adjacent cells around the destroyed brick.
    /// </summary>
    public static void TryShockwave( StatManager statManager, BrickManager brickManager, Vector2Int gridPos, int depth ) {
        float shockwaveChance = statManager.GetStat(UpgradeType.ShockwaveChance);

        float roll = Random.Range(0f, 1f);
        if ( roll >= shockwaveChance ) return;

        Vector2Int[] offsets = {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        };

        foreach ( var offset in offsets ) {
            Vector2Int neighborPos = gridPos + offset;

            if ( brickManager.IsPositionOccupied(neighborPos) ) {
                var neighbor = brickManager.GetBrickAt(neighborPos);
                if ( neighbor != null && !neighbor.IsDead ) {
                    brickManager.RequestDamage(neighbor, 1, DamageSource.Shockwave, null, depth + 1);
                }
            }
        }
    }
}
