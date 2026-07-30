using UnityEngine;

/// <summary>
/// A mobile-safe fog brick. Its visuals are routed through the local pooled VFX library,
/// so this variant no longer requires an Addressables prefab or async load.
/// </summary>
public class FogBrick : MonoBehaviour, IBrickVariant {
    [SerializeField, Min(1)] int initialSmokeCount = 3;
    int smokeDensity;

    public BrickType GetBrickType() => BrickType.Fog;

    public void OnSpawn(BrickScript brickScript) {
        smokeDensity = Mathf.Max(1, initialSmokeCount);
        ArcadeVFXEvent.Raise(new ArcadeVFXRequest(
            ArcadeVFXId.FogPulse, brickScript.transform.position,
            radius: brickScript.SquareSize, intensity: smokeDensity * .25f,
            followTarget: brickScript.transform, loop: true));
    }

    public void OnEndTurn(BrickScript brickScript) {
        smokeDensity++;
        ArcadeVFXEvent.Raise(new ArcadeVFXRequest(
            ArcadeVFXId.FogPulse, brickScript.transform.position,
            radius: brickScript.SquareSize, intensity: Mathf.Clamp(smokeDensity * .18f, .5f, 2f)));
    }

    public void OnHit(BrickScript brickScript) { }
    public void OnDie(BrickScript brickScript) => ArcadeVFXEvent.StopPersistent(brickScript.transform);
}
