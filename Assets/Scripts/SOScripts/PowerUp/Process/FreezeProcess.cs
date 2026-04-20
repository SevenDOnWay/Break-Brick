using UnityEngine;

public class FreezeProcess : Process {

    public override ProcessType GetProssType() => ProcessType.Freeze;

    protected override float GetChance( StatManager statManager ) {
        return statManager.GetStat(UpgradeType.FreezeChance);
    }

    protected override int Execute( StatManager statManager, BrickScript brick, int baseDamage ) {
        int freezeDuration = Mathf.FloorToInt(statManager.GetStat(UpgradeType.FreezeDuration));
        if ( freezeDuration <= 0 ) {
            return 0;
        }

        brick.ApplyOrRefreshEffect(new FreezeEffect(freezeDuration));

        VFXEvent.RaiseVFXCommand(new FreezeVFXCommand(brick.transform.position));

        return 0;
    }
}
