using UnityEngine;

public class ProcessFactory : MonoBehaviour {
    public IProcess CreateProcess( UpgradeType type ) {
        return type switch {
            UpgradeType.ExplosionChance => new ExplosionProcess(),
            UpgradeType.CritChance => new CritProcess(),
            UpgradeType.LightningChance => new LightningProcess(),
            UpgradeType.PoisonChance => new PoisonProcess(),
            UpgradeType.FreezeChance => new FreezeProcess(),
            UpgradeType.SniperInterval => new SniperProcess(),
            UpgradeType.ShockwaveChance => new ShockwaveProcess(),
            UpgradeType.RallyBonus => new RallyProcess(),
            _ => null
        };
    }
}
