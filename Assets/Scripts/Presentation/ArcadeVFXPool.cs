using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Scene-local pool. It has no DI, Addressables, camera, or gameplay-scene dependency.</summary>
public sealed class ArcadeVFXPool : MonoBehaviour {
    [SerializeField] VFXCatalog catalog;
    readonly Dictionary<ArcadeVFXId, Queue<ArcadeVFXPlayer>> available = new();
    readonly Dictionary<ArcadeVFXId, int> created = new();
    readonly Dictionary<ArcadeVFXPlayer, ArcadeVFXId> owners = new();
    public VFXCatalog Catalog => catalog;
    public void SetCatalog(VFXCatalog value) { catalog = value; }

    void OnEnable() { ArcadeVFXEvent.OnRequest += Play; ArcadeVFXEvent.OnStopPersistent += StopPersistent; VFXEvent.OnVFXCommand += PlayLegacy; }
    void OnDisable() { ArcadeVFXEvent.OnRequest -= Play; ArcadeVFXEvent.OnStopPersistent -= StopPersistent; VFXEvent.OnVFXCommand -= PlayLegacy; }
    void Start() => Prewarm();

    public void Prewarm() {
        if (catalog == null) return;
        foreach (var entry in catalog.Entries) for (int i = 0; entry != null && i < entry.defaultPoolSize; i++) Create(entry);
    }

    public void Play(ArcadeVFXRequest request) {
        if (catalog == null || !catalog.TryGet(request.Id, out var entry) || entry.prefab == null) return;
        if (!available.TryGetValue(request.Id, out var queue)) available[request.Id] = queue = new Queue<ArcadeVFXPlayer>();
        ArcadeVFXPlayer player;
        if (queue.Count > 0) {
            player = queue.Dequeue();
        } else {
            if (created.TryGetValue(request.Id, out int count) && count >= entry.maximumConcurrent) return;
            player = Create(entry);
            queue.Dequeue(); // Create registers the instance for prewarm; this checkout owns it now.
        }
        if (player == null) return;
        player.Play(request, Return);
    }

    public void StopPersistent() { foreach (var pair in owners) pair.Key.StopPersistent(); }
    public void StopPersistent(Transform target) { foreach (var pair in owners) if (pair.Key.Follows(target)) pair.Key.StopPersistent(); }
    ArcadeVFXPlayer Create(VFXCatalog.Entry entry) {
        ArcadeVFXPlayer player = Instantiate(entry.prefab, transform); player.gameObject.SetActive(false);
        created.TryGetValue(entry.id, out int count); created[entry.id] = count + 1; owners[player] = entry.id;
        if (!available.TryGetValue(entry.id, out var queue)) available[entry.id] = queue = new Queue<ArcadeVFXPlayer>();
        queue.Enqueue(player); return player;
    }
    void Return(ArcadeVFXPlayer player) {
        if (player == null || !owners.TryGetValue(player, out var id)) return;
        player.gameObject.SetActive(false); available[id].Enqueue(player);
    }
    void PlayLegacy(IVFXCommand command) {
        switch (command) {
            case ExplosionVFXCommand explosion: Play(new ArcadeVFXRequest(ArcadeVFXId.Explosion, explosion.position, radius: explosion.radius)); break;
            case LightningVFXCommand lightning: Play(new ArcadeVFXRequest(ArcadeVFXId.Lightning, lightning.GetStartPos(), endPosition: lightning.GetEndPos())); break;
            case BeamVFXCommand beam: Play(new ArcadeVFXRequest(beam.Axis == Vector2.left || beam.Axis == Vector2.right ? ArcadeVFXId.HorizontalBeam : ArcadeVFXId.VerticalBeam, beam.Position, beam.Axis)); break;
        }
    }
}
