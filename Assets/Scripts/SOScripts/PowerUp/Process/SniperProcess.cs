using UnityEngine;

public class SniperProcess : Process {

    const int SniperMultiplier = 3;

    int hitCounter;

    public override ProcessType GetProssType() => ProcessType.Sniper;

    public override void OnApply() {
        hitCounter = 0;
    }

    public override void Reset() {
        hitCounter = 0;
    }

    // Sniper uses internal hit-counting instead of a probability roll.
    // Always allow Execute to run so the counter advances every hit.
    protected override bool CheckChance( StatManager statManager, BrickScript brick ) {
        return true;
    }

    protected override int Execute( StatManager statManager, BrickScript brick, int baseDamage ) {
        int interval = Mathf.FloorToInt(statManager.GetStat(UpgradeType.SniperInterval));
        if ( interval <= 0 ) {
            interval = 5;
        }

        hitCounter++;

        if ( hitCounter >= interval ) {
            hitCounter = 0;
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.Sniper, brick.transform.position, radius: brick.SquareSize, intensity: 1.25f));
            // Total sniper damage = SniperMultiplier.
            // BallScript applies baseDamage, so return only the bonus on top.
            return SniperMultiplier - baseDamage;
        }

        return 0; // No bonus on non-trigger hits.
    }
}
