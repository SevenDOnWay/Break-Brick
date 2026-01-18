using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class VFXManager : MonoBehaviour {

    UpgradeManager upgradeManager;
    //AudioManager audioManager; //using FMOD


    [Header("SerializeFiled")]
    [SerializeField] ParticleSystem explosionVFXPrefab;


    [Header("List")]
    Queue<ParticleSystem> explosionVFXPool = new Queue<ParticleSystem>();




    [Inject]
    void Constructor( UpgradeManager upgradeManager ) {
        this.upgradeManager = upgradeManager;
    }

    public void Start() {
        SetUpObserver();
        SetUpVFXPool();
    }

    private ParticleSystem AddExplosionVFXToPool() {
        var vfx = Instantiate(explosionVFXPrefab);
        vfx.gameObject.SetActive(false);
        explosionVFXPool.Enqueue(vfx);
        return vfx;
    }

    private void SetUpObserver() {
        //upgradeManager.OnUpgradeAdded += HandleUpgradeAdded;
        //upgradeManager.OnProcessAdded += HandleProcessAdded;
    }

    void SetUpVFXPool() {
        // Pre-instantiate a pool of explosion VFX
        for ( int i = 0; i < 10; i++ ) {
            var vfx = Instantiate(explosionVFXPrefab);
            vfx.gameObject.SetActive(false);
            explosionVFXPool.Enqueue(vfx);
        }
    }


    //void HandleProcessAdded( Process process ) {
    //    // Handle visual effects for process added
    //    if ( process is IVFXEvent vfxSource ) {
    //        vfxSource.RegisterVFXEvents(this);
    //    }
    //}



    public void PlayExplosionVFX( Vector2 position, float radius ) {
        ParticleSystem vfx = GetAvailableExplosionVFX();

        vfx.transform.position = position;
        vfx.transform.localScale = Vector3.one * radius;   // optional
        vfx.gameObject.SetActive(true);

        vfx.Play();
    }

    private ParticleSystem GetAvailableExplosionVFX() {
        int count = explosionVFXPool.Count;

        while ( count-- > 0 ) {
            var vfx = explosionVFXPool.Dequeue();

            // If this particle system is free => use it
            if ( !vfx.isPlaying ) {
                explosionVFXPool.Enqueue(vfx);
                return vfx;
            }

            // Still playing => put back in queue
            explosionVFXPool.Enqueue(vfx);
        }

        // ALL PLAYING => expand pool
        var newVfx = AddExplosionVFXToPool();
        return newVfx;
    }

}
