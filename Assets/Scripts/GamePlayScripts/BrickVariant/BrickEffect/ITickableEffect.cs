
/// <summary>
/// Interface for effects that need to be ticked each turn.
/// Implemented by effects like PoisonEffect and FreezeEffect.
/// </summary>
public interface ITickableEffect : IDurationEffect {
    /// <summary>
    /// Called each turn to update the effect state.
    /// </summary>

    void Tick();

    /// <summary>
    /// Indicates whether the effect is still active.
    /// </summary>
    bool IsActive();
}
