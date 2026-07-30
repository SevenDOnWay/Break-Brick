using UnityEngine;

public class RallyProcess : Process {

    int consecutiveHits;

    public override ProcessType GetProssType() => ProcessType.Rally;

    public override void OnApply() {
        consecutiveHits = 0;
    }

    public override void Reset() {
        consecutiveHits = 0;
    }

    protected override int Execute( StatManager statManager, BrickScript brick, int baseDamage ) {
        float rallyBonus = statManager.GetStat(UpgradeType.RallyBonus);

        consecutiveHits++;
        ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.Rally, brick.transform.position, radius: brick.SquareSize, intensity: Mathf.Clamp(1f + consecutiveHits * .08f, 1f, 2f)));

        // Return only the bonus damage; base damage is applied by BallScript.
        return Mathf.FloorToInt(consecutiveHits * rallyBonus);
    }
}
