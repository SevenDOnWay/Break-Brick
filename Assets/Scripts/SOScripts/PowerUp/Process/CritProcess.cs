using UnityEngine;

public class CritProcess : Process {

    public override ProcessType GetProssType() => ProcessType.Crit;

    // Delegates the chance roll to the base class Template Method.
    protected override float GetChance( StatManager statManager ) {
        return statManager.GetStat(UpgradeType.CritChance);
    }

    protected override int Execute( StatManager statManager, BrickScript brick, int baseDamage ) {
        float critMultiplier = statManager.GetStat(UpgradeType.CritMultiplier);

        // Total crit damage = floor(baseDamage * critMultiplier).
        // BallScript already applies baseDamage, so return only the bonus on top.
        int temp  =Mathf.FloorToInt(baseDamage * critMultiplier) - baseDamage;
        ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.Crit, brick.transform.position, radius: brick.SquareSize));
        Debug.Log($"CritMultiplier: {critMultiplier}, BaseDamage: {baseDamage}, BonusDamage: {temp}");
        return temp;
    }
}
