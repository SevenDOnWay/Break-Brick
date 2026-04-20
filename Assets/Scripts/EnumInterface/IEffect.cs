using System;
using UnityEngine;

public interface IEffect  {
    EffectType Type { get; }

    void OnApply( BrickScript brick );
    void OnRemove( BrickScript brick );

    void Refresh( IEffect newEffect );
}

public interface IDurationEffect : IEffect {
    int RemainingDuration { get; }
    bool IsExpired { get; }
}

public interface IStackableEffect : IEffect {
    int Stacks { get; }
    int MaxStacks { get; }
    bool CanStackWith( IEffect effect );
    void AddStack( IEffect effect );
}
