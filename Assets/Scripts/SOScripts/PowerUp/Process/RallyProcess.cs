using UnityEngine;

public class RallyProcess : Process {

    int consecutiveHits;

    public override ProcessType GetProssType() => ProcessType.Rally;

    public override int OnHit( StatManager statManager, BrickScript brick ) {
        float rallyBonus = statManager.GetStat(UpgradeType.RallyBonus);

        consecutiveHits++;

        int bonusDamage = Mathf.FloorToInt(consecutiveHits * rallyBonus);

        return 1 + bonusDamage;
    }

    public override void OnApply() {
        consecutiveHits = 0;
    }

    public void ResetCounter() {
        consecutiveHits = 0;
    }
}
