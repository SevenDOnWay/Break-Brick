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
    public static void TryShockwave( StatManager statManager, BrickManager brickManager, BrickScript origin, int depth ) {
        float shockwaveChance = statManager.GetStat(UpgradeType.ShockwaveChance);

        float roll = Random.Range(0f, 1f);
        if ( roll >= shockwaveChance ) return;

        brickManager?.RequestOrthogonalDamage(origin, 1, DamageSource.Shockwave, depth + 1);
    }
}
