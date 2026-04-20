using UnityEngine;

public abstract class Process : IProcess {

    public abstract ProcessType GetProssType();

    public virtual int OnHit( StatManager statManager, BrickScript brick, int baseDamage ) {
        if ( brick == null || brick.IsDead || statManager == null ) {
            return 0;
        }

        if ( !CheckChance(statManager, brick) ) {
            return 0;
        }

        return Execute(statManager, brick, baseDamage);
    }

    public virtual void OnApply() { }

    public virtual void Reset() { }

    protected virtual bool CheckChance( StatManager statManager, BrickScript brick ) {
        return RollChance(GetChance(statManager));
    }

    protected virtual float GetChance( StatManager statManager ) {
        return 1f;
    }

    protected virtual int Execute( StatManager statManager, BrickScript brick, int baseDamage ) {
        return 0;
    }

    protected bool RollChance( float chance ) {
        return Random.value < Mathf.Clamp01(chance);
    }

}
