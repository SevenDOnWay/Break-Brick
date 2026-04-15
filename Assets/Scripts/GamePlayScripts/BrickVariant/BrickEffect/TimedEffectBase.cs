using UnityEngine;

public abstract class TimedEffectBase : ITickableEffect {
    protected BrickScript owner;
    protected int remainingDuration;

    public abstract EffectType Type { get; }
    public int RemainingDuration => remainingDuration;
    public bool IsExpired => remainingDuration <= 0;

    protected TimedEffectBase( int duration ) {
        remainingDuration = Mathf.Max(0, duration);
    }

    public virtual void OnApply( BrickScript brick ) {
        owner = brick;
    }

    public virtual void OnRemove( BrickScript brick ) {
        if ( owner == brick ) {
            owner = null;
        }
    }

    public virtual void Refresh( IEffect newEffect ) {
        if ( newEffect is IDurationEffect durationEffect ) {
            remainingDuration = Mathf.Max(remainingDuration, durationEffect.RemainingDuration);
        }
    }

    public virtual void Tick() {
        if ( remainingDuration > 0 ) {
            remainingDuration--;
        }
    }

    public virtual bool IsActive() {
        return owner != null && !owner.IsDead && !IsExpired;
    }
}
