using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Standalone Testing Particle scene controller. Uses the catalog/pool directly.</summary>
public sealed class VFXPreviewRunner : MonoBehaviour {
    [SerializeField] VFXCatalog catalog;
    [SerializeField] ArcadeVFXPool pool;
    [SerializeField] Transform[] previewTargets;
    int selected;
    float radius = 1f, intensity = 1f;
    Vector2 direction = Vector2.right;
    bool loop;
    int seed = 12345;

    void Reset() { pool = GetComponent<ArcadeVFXPool>(); }
    void Awake() { if (pool == null) pool = GetComponent<ArcadeVFXPool>(); if (catalog != null && pool != null) pool.SetCatalog(catalog); }
    void Update() { if (Input.GetKeyDown(KeyCode.Space)) Replay(); if (Input.GetKeyDown(KeyCode.R)) { seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue); Replay(); } }
    public void Replay() {
        if (catalog == null || catalog.Entries.Count == 0 || pool == null) return;
        selected = Mathf.Clamp(selected, 0, catalog.Entries.Count - 1);
        var entry = catalog.Entries[selected];
        List<Vector3> targets = new();
        if (previewTargets != null) foreach (var target in previewTargets) if (target != null) targets.Add(target.position);
        Vector3 position = transform.position;
        Vector3 end = position + (Vector3)direction.normalized * Mathf.Max(1f, radius * 2f);
        pool.Play(new ArcadeVFXRequest(entry.id, position, direction, end, radius, intensity, seed, targets, null, loop));
    }
    void OnGUI() {
        if (catalog == null || catalog.Entries.Count == 0) return;
        GUILayout.BeginArea(new Rect(12, 12, 270, 360), GUI.skin.box);
        GUILayout.Label("Portable Arcade VFX Preview");
        string[] names = new string[catalog.Entries.Count]; for (int i = 0; i < names.Length; i++) names[i] = catalog.Entries[i].id.ToString();
        selected = GUILayout.SelectionGrid(selected, names, 2);
        GUILayout.Label($"Radius {radius:F2}"); radius = GUILayout.HorizontalSlider(radius, .25f, 4f);
        GUILayout.Label($"Intensity {intensity:F2}"); intensity = GUILayout.HorizontalSlider(intensity, .25f, 3f);
        loop = GUILayout.Toggle(loop, "Loop / persistent");
        if (GUILayout.Button("Replay (Space)")) Replay();
        if (GUILayout.Button("Randomize seed (R)")) { seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue); Replay(); }
        if (GUILayout.Button("Replay all")) { for (int i = 0; i < catalog.Entries.Count; i++) { selected = i; Replay(); } }
        if (GUILayout.Button("Stop looping effects")) pool.StopPersistent();
        GUILayout.EndArea();
    }
}
