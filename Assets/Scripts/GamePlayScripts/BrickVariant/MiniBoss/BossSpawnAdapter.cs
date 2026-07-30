using UnityEngine;

/// <summary>
/// Optional boss-side adapter for changing normal wave spawns while the boss is alive.
/// Attach it to a boss prefab and configure its effect in the inspector.
/// </summary>
public sealed class BossSpawnAdapter : MonoBehaviour {
    [SerializeField, Min(0.1f)] float waveBudgetMultiplier = 1.25f;
    [SerializeField] bool replaceAllSpawnedBricks;
    [SerializeField] BrickType replacementBrickType;

    public float ModifyWaveBudget( float budget ) {
        return budget * waveBudgetMultiplier;
    }

    public bool TryGetReplacementBrickType( out BrickType brickType ) {
        brickType = replacementBrickType;
        return replaceAllSpawnedBricks;
    }
}
