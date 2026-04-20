using VContainer;

public class ProcessFactory {
    readonly IObjectResolver resolver;

    public ProcessFactory( IObjectResolver resolver ) {
        this.resolver = resolver;
    }

    public Process CreateProcess( UpgradeType type ) {
        Process process = type switch {
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

        if ( process != null ) {
            resolver.Inject(process);
        }

        return process;
    }
}
