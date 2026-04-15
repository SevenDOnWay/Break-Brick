using UnityEngine;

public class CritProcess : Process {
    public override ProcessType GetProssType() => ProcessType.Crit;

    public override int OnHit( StatManager statManager, BrickScript brick ) {

        float critChance = statManager.GetStat( UpgradeType.CritChance );
        float CritMultiplier = statManager.GetStat( UpgradeType.CritMultiplier );

        float roll = Random.Range(0f, 1f);

        if ( roll <= critChance ) return (int)Mathf.Floor(1 * CritMultiplier);

        return 2; //return double dame for now
    }


}
