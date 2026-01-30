using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

public class BrickScript : MonoBehaviour {
    LevelManager levelManager;
    
    public bool IsDead => health <= 0;
    public int health;
    public Vector2Int GridPosition { get; set; }

    [SerializeField] TextMeshPro healText;
    SpriteRenderer spriteRenderer;

    IBrickVariant[] variants;

    //TODO: Find a better way to manage colors, or better color scheme
    Dictionary<int, string> colors = new Dictionary<int, string>{
            { 0, "#3939CC" },
            { 25, "#49C5CC" },
            { 50, "#45CC45" },
            { 75, "#E6E077" },
            { 100, "#E6463E" }
    };

    //CONSIDER: Delete this change if not needed
    public static event EventHandler OnBrickDestroyed;
    public static event EventHandler OnBrickHit;

    public event Action<BrickScript, DamageSource, int> OnHit;
    public event Action<DamageRequest> OnDamaged;
    public event Action<Vector2Int> OnDestroyed;

    [Inject]
    public void Constructor( LevelManager levelManager ) {
        this.levelManager = levelManager;
    }

    public void Init( int health, BrickManager brickManager ) {
        this.health = health;
        variants = GetComponents<IBrickVariant>();
    }

    /// <summary>
    /// Method to notify the brick that it has been hit by a damage source.
    /// Call this method order to trigger OnHit event.
    /// </summary>
    /// <param name="source"></param>
    public void NotifyHit( DamageSource source, int damage = 1) {
        OnHit?.Invoke(this, source, damage);
    }
    

    public void Init( int health ) {
        this.health = health;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if ( spriteRenderer == null ) Debug.LogError("SpriteRenderer is null in BrickScript.");

        variants = GetComponents<IBrickVariant>();

        foreach ( var variant in variants ) {
            try {
                variant.OnSpawn(this);
            }
            catch ( Exception e ) {
                Debug.LogException(e);
            }
        }

        UpdateBrickVisual();
    }

    public void ApplyDamageInternal( DamageRequest req ) {
        //Debug.Log($"Taking damage: {damage}");
        int damage = req.damage;

        health -= damage;

        UpdateBrickVisual();

        levelManager.AddExp(damage);

        if ( health > 0 ) {
            OnBrickHit.Invoke(this, EventArgs.Empty);
            OnDamage(req);
        }
        else {
            OnDeath(req);
        }
    }

    #region Update Visuals
    void UpdateBrickVisual() {
        UpdateHealthText();
        UpdateColor();
    }

    public void UpdateHealthText() {
        healText.text = health.ToString();
    }

    void UpdateColor() {
        List<int> keys = new List<int>(colors.Keys);
        keys.Sort();

        for ( int i = 0; i < keys.Count - 1; i++ ) {
            int lowerKey = keys[i];
            int upperKey = keys[i + 1];

            if ( health >= lowerKey && health <= upperKey ) {
                Color lowerColor = ConvertStringToHex(colors[lowerKey]);
                Color upperColor = ConvertStringToHex(colors[upperKey]);

                float t = (health - lowerKey) / (float)(upperKey - lowerKey);

                Color lerpedColor = Color.Lerp(lowerColor, upperColor, t);
                spriteRenderer.color = lerpedColor;

                break;
            }

        }
    }
    #endregion

    void OnDamage(DamageRequest req) {
        foreach ( var variant in variants ) {
            try {
                variant.OnHit(this);
            }
            catch ( Exception e ) {
                Debug.LogException(e);
            }
        }
    }

    void OnDeath(DamageRequest req ) {
        foreach ( var variant in variants ) {
            try {
                variant.OnDie(this);
            }
            catch ( Exception e ) {
                Debug.LogException(e);
            }
        }

        gameObject.SetActive(false);
        DestroyBrick();
    }

    void DestroyBrick() {
        OnDestroyed?.Invoke(GridPosition);
        Destroy(gameObject);
    }

    public Color ConvertStringToHex( string hex ) {
        if ( UnityEngine.ColorUtility.TryParseHtmlString(hex, out var color) )
            return color;

        Debug.LogWarning($"Invalid hex color: {hex}");
        return Color.magenta;
    }

    public void UpdateGridPosition(Vector2Int pos) {
        GridPosition = pos;
    }

    //private void OnTriggerEnter2D( Collider2D collision ) {
    //    if ( collision.CompareTag("EndLine") ) {
    //        Debug.Log("Brick hit the end line!");
    //    }
    //}


}
