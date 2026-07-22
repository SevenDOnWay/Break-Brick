public class FreezeEffect : TimedEffectBase {
    public override EffectType Type => EffectType.Freeze;

    public FreezeEffect( int duration ) : base(duration) { }

    public override void OnApply( IEffectTarget target ) {
        base.OnApply(target);
    }

    public override void Tick() {
        base.Tick();
    }

    public bool IsFrozen() => !IsExpired;
}
