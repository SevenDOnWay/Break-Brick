using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Brick variant that surrounds itself with an accumulating smoke overlay.
/// On spawn it loads a smoke prefab via <b>Addressables</b> and immediately instantiates
/// <see cref="initialSmokeCount"/> child objects. Each subsequent turn one more smoke is added.
/// When the brick dies all smoke instances are destroyed and the Addressable handle is released.
/// </summary>
/// <remarks>
/// The smoke prefab must be labelled / registered in Addressables and its GUID assigned to
/// <see cref="smokePrefabRef"/> in the Inspector. The prefab should contain a
/// <see cref="SpriteRenderer"/> whose colour will be tinted by <see cref="smokeTint"/>.
/// </remarks>
public class FogBrick : MonoBehaviour, IBrickVariant {
    [SerializeField] AssetReferenceGameObject smokePrefabRef;
    [SerializeField] int initialSmokeCount = 3;
    [SerializeField] Color smokeTint = new Color(0.5f, 0.5f, 0.5f, 0.6f);

    const float SmokeScatterRadius = 0.3f;

    readonly List<GameObject> smokeInstances = new();
    AsyncOperationHandle<GameObject> loadHandle;
    bool isLoaded;
    BrickScript owner;

    // ── IBrickVariant ──────────────────────────────────────────────────────────

    public BrickType GetBrickType() => BrickType.Fog;

    public void OnSpawn( BrickScript brickScript ) {
        owner = brickScript;
        LoadAndSpawn(initialSmokeCount);
    }

    public void OnEndTurn( BrickScript brickScript ) {
        if ( !isLoaded || !loadHandle.IsValid() ) return;
        SpawnSmoke(loadHandle.Result);
    }

    public void OnHit( BrickScript brickScript ) { }

    public void OnDie( BrickScript brickScript ) {
        foreach ( var smoke in smokeInstances ) {
            if ( smoke != null ) Destroy(smoke);
        }
        smokeInstances.Clear();

        if ( loadHandle.IsValid() ) {
            Addressables.Release(loadHandle);
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    void LoadAndSpawn( int count ) {
        loadHandle = smokePrefabRef.LoadAssetAsync<GameObject>();
        loadHandle.Completed += handle => {
            if ( handle.Status != AsyncOperationStatus.Succeeded ) {
                Debug.LogError("[FogBrick] Failed to load smoke prefab.");
                return;
            }

            isLoaded = true;

            for ( int i = 0; i < count; i++ ) {
                SpawnSmoke(handle.Result);
            }
        };
    }

    void SpawnSmoke( GameObject prefab ) {
        if ( owner == null || owner.IsDead ) return;

        var smoke = Instantiate(prefab, owner.transform);
        smoke.transform.localPosition = (Vector3)(Random.insideUnitCircle * SmokeScatterRadius);

        if ( smoke.TryGetComponent<SpriteRenderer>(out var spriteRenderer) ) {
            spriteRenderer.color = smokeTint;
        }

        smokeInstances.Add(smoke);
    }
}
