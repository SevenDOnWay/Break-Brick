using UnityEngine;

public class SniperProcess : Process {

    int hitCounter;

    public override ProcessType GetProssType() => ProcessType.Sniper;

    public override int OnHit( StatManager statManager, BrickScript brick ) {
        int interval = Mathf.FloorToInt(statManager.GetStat(UpgradeType.SniperInterval));
        if ( interval <= 0 ) interval = 5;

        hitCounter++;

        if ( hitCounter >= interval ) {
            hitCounter = 0;
            const int sniperMultiplier = 3;
            return sniperMultiplier;
        }

        return 1;
    }

    public override void OnApply() {
        hitCounter = 0;
    }

    public void ResetCounter() {
        hitCounter = 0;
    }
}
