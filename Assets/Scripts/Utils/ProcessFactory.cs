using VContainer;

public class ProcessFactory {
    readonly IObjectResolver resolver;

    public ProcessFactory( IObjectResolver resolver ) {
        this.resolver = resolver;
    }

    public Process CreateProcess( ProcessType type ) {
        Process process = type switch {
            ProcessType.Explosion => new ExplosionProcess(),
            ProcessType.Crit => new CritProcess(),
            ProcessType.Lightning => new LightningProcess(),
            ProcessType.Poison => new PoisonProcess(),
            ProcessType.Freeze => new FreezeProcess(),
            ProcessType.Sniper => new SniperProcess(),
            ProcessType.Shockwave => new ShockwaveProcess(),
            ProcessType.Rally => new RallyProcess(),
            _ => null
        };

        if ( process != null ) {
            resolver.Inject(process);
        }

        return process;
    }
}
