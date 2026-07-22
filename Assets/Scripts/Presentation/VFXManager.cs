using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class VFXManager : MonoBehaviour {

    [Inject] IObjectResolver resolver;

    [Serializable]
    public class VFXPlayer {
        public VFXType type;
        public VFXPlayerBase prefab;
        public int prewarmCount = 10;
    }


    [SerializeField] List<VFXPlayer> players;

    Dictionary<VFXType,Queue<VFXPlayerBase>> playersMap = new();
    Dictionary<VFXType, VFXPlayer> settingsLookup = new();
    Dictionary<VFXType, Transform> parentMap = new();

    public void OnEnable() {
        VFXEvent.OnVFXCommand += HandleVFX;
        //Debug.Log(" VFX Manager Enabled and listening to VFX Events ");
    }

    public void OnDisable() {
        VFXEvent.OnVFXCommand -= HandleVFX;
    }


    public void Start() {
        SetUpVFXPool();
    }

    void SetUpVFXPool() {
        foreach ( var p in players ) {
            if ( p.prefab == null ) {
                Debug.LogWarning(" VFX Player " + p.type + " prefab is null ");
                continue;
            }

            // map vfxtype to setting 
            playersMap[p.type] = new Queue<VFXPlayerBase>();
            settingsLookup[p.type] = p;

            // create parent folder
            Transform folder = new GameObject($"Pool_{p.type}").transform;
            folder.transform.SetParent(this.transform);
            parentMap[p.type] = folder;

            for ( int i = 0; i < p.prewarmCount; i++ ) {
                CreateVFXPlayer(p.type);
            }
        }
    }

    public void HandleVFX( IVFXCommand cmd ) {
        VFXType type = cmd.GetVFXType();

        Debug.Log(" [VFX Manager] received VFX Command of type " + type);

        if ( !playersMap.TryGetValue(type, out var queue) ) {
            Debug.LogWarning(" VFX Type " + type + " not found in players map ");
            return;
        }

        if ( queue.Count == 0 ) {
            CreateVFXPlayer(type);
        }

        VFXPlayerBase player = queue.Dequeue();

        player.Execute(cmd, () => ReturnToPool(type, player));
    }

    private void ReturnToPool( VFXType type, VFXPlayerBase player ) {
        player.gameObject.SetActive(false);
        playersMap[type].Enqueue(player);
    }

    private void AddVFXComponent( VFXType type) {
        CreateVFXPlayer(type);
    }

    /// <summary>
    /// take VFXType then create it and add to pool
    /// </summary>
    /// <param name="player"> the player it take </param>
    /// <param name="time"></param>
    public void CreateVFXPlayer(VFXType type ) {
        if ( !settingsLookup.TryGetValue(type, out var settings) ) return;
        if ( !parentMap.TryGetValue(type, out var parentTransform) ) return;

        var obj = settings.prefab;

        if ( obj == null ) {
            Debug.LogWarning(" VFX Player " + type + " prefab is null ");
        }


        var playerInterface = Instantiate(obj, parentTransform);
        resolver.Inject(playerInterface);

        playerInterface.gameObject.SetActive(false);
        playersMap[type].Enqueue(playerInterface);

    }


    //private ParticleSystem AddExplosionVFXToPool() {
    //    var vfx = Instantiate(explosionVFXPrefab);
    //    vfx.gameObject.SetActive(false);
    //    explosionVFXPool.Enqueue(vfx);
    //    return vfx;
    //}


    //void HandleProcessAdded( Process process ) {
    //    // Handle visual effects for process added
    //    if ( process is IVFXEvent vfxSource ) {
    //        vfxSource.RegisterVFXEvents(this);
    //    }
    //}



    //public void PlayExplosionVFX( Vector2 position, float radius ) {
    //    ParticleSystem vfx = GetAvailableExplosionVFX();

    //    vfx.transform.position = position;
    //    vfx.transform.localScale = Vector3.one * radius;   // optional
    //    vfx.gameObject.SetActive(true);

    //    vfx.Play();
    //}

    //private ParticleSystem GetAvailableExplosionVFX() {
    //    int count = explosionVFXPool.Count;

    //    while ( count-- > 0 ) {
    //        var vfx = explosionVFXPool.Dequeue();

    //        // If this particle system is free => use it
    //        if ( !vfx.isPlaying ) {
    //            explosionVFXPool.Enqueue(vfx);
    //            return vfx;
    //        }

    //        // Still playing => put back in queue
    //        explosionVFXPool.Enqueue(vfx);
    //    }

    //    // ALL PLAYING => expand pool
    //    var newVfx = AddExplosionVFXToPool();
    //    return newVfx;
    //}

}
