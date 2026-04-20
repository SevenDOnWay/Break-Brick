/// <summary>
/// Implemented by brick variants that can silently cancel incoming damage.
/// Checked in <see cref="BrickScript.ApplyDamageInternal"/> before any HP reduction occurs.
/// </summary>
public interface IDamageBlocker {
    /// <summary>
    /// Return <c>true</c> to fully cancel the damage request; <c>false</c> to let it pass through.
    /// </summary>
    bool TryBlock( DamageRequest req );
}
