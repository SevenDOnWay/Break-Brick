using UnityEngine;

public class PoisonProcess : Process {

    public override ProcessType GetProssType() => ProcessType.Poison;

    protected override float GetChance( StatManager statManager ) {
        return statManager.GetStat(UpgradeType.PoisonChance);
    }

    protected override int Execute( StatManager statManager, BrickScript brick, int baseDamage ) {
        int poisonDuration = Mathf.FloorToInt(statManager.GetStat(UpgradeType.PoisonDuration));
        if ( poisonDuration <= 0 ) {
            return 0;
        }

        brick.ApplyOrRefreshEffect(new PoisonEffect(poisonDuration));

        VFXEvent.RaiseVFXCommand(new PoisonVFXCommand(brick.transform.position));

        return 0;
    }
}
