using UnityEngine;

/// <summary>
/// Minimal gameplay surface that an effect may act upon. This keeps effect
/// contracts independent from the concrete BrickScript implementation.
/// </summary>
public interface IEffectTarget {
    bool IsDead { get; }

    void NotifyHit( DamageSource source, int damage = 1, Vector2 hitNormal = default );
}

public interface IEffect  {
    EffectType Type { get; }

    void OnApply( IEffectTarget target );
    void OnRemove( IEffectTarget target );

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
