using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

public class BrickScript : MonoBehaviour, IEffectTarget {
    [Serializable]
    public struct EffectLayerBinding {
        public EffectType effectType;
        public GameObject layerObject;
    }

    static readonly int[] ColorThresholds = { 0, 25, 50, 75, 100 };
    static readonly Color[] CachedColors = {
        new Color32(57, 57, 204, 255),
        new Color32(73, 197, 204, 255),
        new Color32(69, 204, 69, 255),
        new Color32(230, 224, 119, 255),
        new Color32(230, 70, 62, 255)
    };

    LevelManager levelManager;
    RunDataManager runDataManager;

    public bool IsDead => health <= 0;
    public bool IsRestoringSavedHealth { get; private set; }
    public int health;
    public Vector2Int GridPosition { get; set; }
    public float SquareSize { get; private set; }

    [Header("Effect Layer")]
    [SerializeField] public List<EffectLayerBinding> effectLayerBindings = new();

    [SerializeField] TextMeshPro healText;
    SpriteRenderer spriteRenderer;
    IBrickVariant[] variants;

    readonly Dictionary<EffectType, IEffect> activeEffects = new();
    readonly List<ITickableEffect> tickableEffects = new();
    readonly List<EffectType> pendingRemoval = new();
    readonly Dictionary<EffectType, GameObject> effectLayerMap = new();

    public event Action<BrickScript, DamageSource, int, Vector2> OnHit;
    public event Action<DamageRequest> OnDamaged;
    public event Action<Vector2Int> OnDestroyed;
    public event Action<EffectType, bool> OnEffectChanged;

    [Inject]
    public void Constructor( LevelManager levelManager, RunDataManager runDataManager ) {
        this.levelManager = levelManager;
        this.runDataManager = runDataManager;
    }

    public void Init( int health, float squareSize, bool isRestoringSavedHealth = false ) {
        this.health = health;
        IsRestoringSavedHealth = isRestoringSavedHealth;
        this.SquareSize = squareSize;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        variants = GetComponents<IBrickVariant>();

        SubscribeEffectLayer(null, false);

        if ( spriteRenderer == null ) {
            Debug.LogError("SpriteRenderer is null in BrickScript.");
        }

        try {
            foreach ( var variant in variants ) {
                try {
                    variant.OnSpawn(this);
                } catch ( Exception e ) {
                    Debug.LogException(e);
                }
            }
        } finally {
            IsRestoringSavedHealth = false;
        }

        UpdateBrickVisual();
    }

    public void NotifyHit( DamageSource source, int damage = 1, Vector2 hitNormal = default ) {
        OnHit?.Invoke(this, source, damage, hitNormal);
    }

    public void SubscribeEffectLayer( IEffect effect, bool isActive ) {
        if ( effectLayerMap.Count == 0 ) {
            foreach ( var binding in effectLayerBindings ) {
                if ( binding.layerObject == null ) continue;
                effectLayerMap[binding.effectType] = binding.layerObject;
                binding.layerObject.SetActive(false);
            }
        }

        if ( effect != null ) {
            UpdateEffectLayer(effect.Type, isActive);
        }
    }

    public void ApplyDamageInternal( DamageRequest req ) {
        foreach ( var variant in variants ) {
            if ( variant is IDamageBlocker blocker && blocker.TryBlock(req) ) {
                ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.ShieldBlock, transform.position, req.hitNormal, radius: SquareSize * .5f));
                return;
            }
        }

        int damage = req.damage;
        int dealtDamage = Mathf.Min(Mathf.Max(0, damage), Mathf.Max(0, health));
        health -= damage;

        if ( req.source == DamageSource.Piercing ) {
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.PiercingImpact, transform.position, req.hitNormal, radius: SquareSize));
        } else if ( req.source == DamageSource.Heavy ) {
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.HeavyImpact, transform.position, req.hitNormal, radius: SquareSize));
        }

        GameplayEvents.RaiseBrickEvent(BrickEventType.Hit);
        OnDamaged?.Invoke(req);

        UpdateBrickVisual();
        levelManager.AddExp(damage);
        runDataManager?.runData?.RecordDamage(req.source, dealtDamage);

        if ( health > 0 ) {
            OnDamage(req);
        } else {
            OnDeath(req);
        }
    }

    public void TickEffects() {
        pendingRemoval.Clear();

        for ( int i = 0; i < tickableEffects.Count; i++ ) {
            ITickableEffect effect = tickableEffects[i];
            effect.Tick();

            if ( !effect.IsActive() || effect.IsExpired ) {
                pendingRemoval.Add(effect.Type);
            }
        }

        for ( int i = 0; i < pendingRemoval.Count; i++ ) {
            RemoveEffect(pendingRemoval[i]);
        }
    }

    public bool HasActiveEffect( EffectType type ) {
        return activeEffects.ContainsKey(type);
    }

    void UpdateBrickVisual() {
        UpdateHealthText();
        UpdateColor();
        UpdateEffectLayer();
    }

    public void UpdateHealthText() {
        if ( healText != null ) {
            healText.text = health.ToString();
        }
    }

    void UpdateColor() {
        if ( spriteRenderer == null ) return;

        if ( health <= ColorThresholds[0] ) {
            spriteRenderer.color = CachedColors[0];
            return;
        }

        int lastIndex = ColorThresholds.Length - 1;
        if ( health >= ColorThresholds[lastIndex] ) {
            spriteRenderer.color = CachedColors[lastIndex];
            return;
        }

        for ( int i = 0; i < lastIndex; i++ ) {
            int lower = ColorThresholds[i];
            int upper = ColorThresholds[i + 1];
            if ( health < lower || health > upper ) continue;

            float t = (health - lower) / (float)(upper - lower);
            spriteRenderer.color = Color.Lerp(CachedColors[i], CachedColors[i + 1], t);
            return;
        }
    }

    void UpdateEffectLayer() {
        foreach ( var pair in effectLayerMap ) {
            bool isActive = activeEffects.ContainsKey(pair.Key);
            if ( pair.Value.activeSelf != isActive ) {
                pair.Value.SetActive(isActive);
            }
        }
    }

    void UpdateEffectLayer( EffectType effectType, bool isActive ) {
        if ( effectLayerMap.TryGetValue(effectType, out var layer) && layer != null ) {
            layer.SetActive(isActive);
        }
    }

    void OnDamage( DamageRequest req ) {
        foreach ( var variant in variants ) {
            try {
                variant.OnHit(this);
            } catch ( Exception e ) {
                Debug.LogException(e);
            }
        }
    }

    void OnDeath( DamageRequest req ) {
        runDataManager?.runData?.RecordBrickDestroyed();

        foreach ( var variant in variants ) {
            try {
                variant.OnDie(this);
            } catch ( Exception e ) {
                Debug.LogException(e);
            }
        }

        for ( int i = tickableEffects.Count - 1; i >= 0; i-- ) {
            RemoveEffect(tickableEffects[i].Type);
        }

        gameObject.SetActive(false);
        DestroyBrick();
    }

    void DestroyBrick() {
        OnDestroyed?.Invoke(GridPosition);
        GameplayEvents.RaiseBrickEvent(BrickEventType.Destroyed);
        Destroy(gameObject);
    }

    public void UpdateGridPosition( Vector2Int pos ) {
        GridPosition = pos;
    }

    public void CallVariantEndTurn() {
        foreach ( var variant in variants ) {
            try {
                variant.OnEndTurn(this);
            } catch ( Exception e ) {
                Debug.LogException(e);
            }
        }
    }

    public void ApplyOrRefreshEffect( IEffect newEffect ) {
        if ( newEffect == null ) return;

        if ( activeEffects.TryGetValue(newEffect.Type, out var existingEffect) ) {
            existingEffect.Refresh(newEffect);
            if ( existingEffect is ITickableEffect tickable && !tickableEffects.Contains(tickable) ) {
                tickableEffects.Add(tickable);
            }
            UpdateEffectLayer(existingEffect.Type, true);
            return;
        }

        activeEffects[newEffect.Type] = newEffect;
        newEffect.OnApply(this);

        if ( newEffect.Type == EffectType.Freeze ) {
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.FreezeApply, transform.position, radius: SquareSize));
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.FreezeLoop, transform.position, radius: SquareSize, followTarget: transform, loop: true));
        } else if ( newEffect.Type == EffectType.Poison ) {
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.PoisonApply, transform.position, radius: SquareSize));
            ArcadeVFXEvent.Raise(new ArcadeVFXRequest(ArcadeVFXId.PoisonLoop, transform.position, radius: SquareSize, followTarget: transform, loop: true));
        }

        if ( newEffect is ITickableEffect tickableEffect ) {
            tickableEffects.Add(tickableEffect);
        }

        OnEffectChanged?.Invoke(newEffect.Type, true);
        UpdateEffectLayer(newEffect.Type, true);
    }

    void RemoveEffect( EffectType effectType ) {
        if ( !activeEffects.TryGetValue(effectType, out var effect) ) {
            return;
        }

        effect.OnRemove(this);
        if ( effectType == EffectType.Freeze || effectType == EffectType.Poison ) {
            ArcadeVFXEvent.StopPersistent(transform);
        }
        activeEffects.Remove(effectType);

        if ( effect is ITickableEffect tickable ) {
            tickableEffects.Remove(tickable);
        }

        OnEffectChanged?.Invoke(effectType, false);
        UpdateEffectLayer(effectType, false);
    }
}
