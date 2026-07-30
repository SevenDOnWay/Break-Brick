using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Break Brick/VFX Catalog", fileName = "VFXCatalog")]
public sealed class VFXCatalog : ScriptableObject {
    [Serializable]
    public sealed class Entry {
        public ArcadeVFXId id;
        public ArcadeVFXPlayer prefab;
        [Min(0)] public int defaultPoolSize = 2;
        [Min(1)] public int maximumConcurrent = 6;
        [Min(0.01f)] public float previewRadius = 1f;
        [Min(0.01f)] public float previewIntensity = 1f;
    }

    [SerializeField] List<Entry> entries = new();
    public IReadOnlyList<Entry> Entries => entries;

    public bool TryGet(ArcadeVFXId id, out Entry entry) {
        for (int i = 0; i < entries.Count; i++) {
            if (entries[i] != null && entries[i].id == id) { entry = entries[i]; return true; }
        }
        entry = null;
        return false;
    }
}
