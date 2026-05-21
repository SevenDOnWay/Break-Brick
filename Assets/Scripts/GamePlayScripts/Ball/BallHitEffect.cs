using UnityEngine;

public abstract class BallHitEffect : ScriptableObject {
    public abstract BallType BallType { get; }

    public void Apply( BallHitContext context ) {
        if ( context.brick == null || context.brick.IsDead ) {
            return;
        }

        if ( !context.TryMarkEffectTriggered(this) ) {
            return;
        }

        Execute(context);
    }

    protected abstract void Execute( BallHitContext context );
}
