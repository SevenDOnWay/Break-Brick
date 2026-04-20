public class PoisonEffect : TimedEffectBase {
    readonly int damagePerTick;

    public override EffectType Type => EffectType.Poison;

    public PoisonEffect( int duration, int damagePerTick = 1 ) : base(duration) {
        this.damagePerTick = damagePerTick;
    }

    public override void Tick() {
        if ( owner == null || owner.IsDead || IsExpired ) {
            return;
        }

        owner.NotifyHit(DamageSource.Poison, damagePerTick);
        base.Tick();
    }
}
