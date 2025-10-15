using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

public class BrickScript : MonoBehaviour {
    public int health;
    [Inject] LevelManager levelManager;

    [SerializeField] TextMeshPro healText;
    SpriteRenderer spriteRenderer;

    //TODO: Find a better way to manage colors, and better color scheme
    Dictionary<int, string> colors = new Dictionary<int, string>{
            { 0, "#FFFFFF" },
            { 25, "#3939CC" },
            { 50, "#49C5CC" },
            { 75, "#45CC45" },
            { 100, "#E6E077" },
            { 150, "#E6463E" }
    };

    
    public static event EventHandler OnBrickDestroyed;
    public static event EventHandler OnBrickHit;

    public void Init( int health ) {
        this.health = health;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if ( spriteRenderer == null ) Debug.LogError("SpriteRenderer is null in BrickScript.");

        foreach ( var variant in GetComponents<IBrickVariant>() ) {
            try {
                variant.OnSpawn(this);
            }
            catch ( Exception e ) {
                Debug.LogException(e);
            }
        }

        UpdateBrickVisual();
    }

    public void TakeDamage( int damage ) {
        //Debug.Log($"Taking damage: {damage}");
        health -= damage;

        OnBrickHit.Invoke(this, EventArgs.Empty);

        levelManager.AddExp(damage);
        UpdateBrickVisual();

        if ( health <= 0 ) {
            Debug.Log($"will be destroyed");
            DestroyBrick();
        }


    }

    public void UpdateBrickVisual() {
        UpdateHealthText();
        UpdateColor();
    }

    public void UpdateHealthText() {
        healText.text = health.ToString();
    }

    public void UpdateColor() {
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

    public static Color ConvertStringToHex
        ( string hex ) {
        if ( UnityEngine.ColorUtility.TryParseHtmlString(hex, out var color) )
            return color;

        Debug.LogWarning($"Invalid hex color: {hex}");
        return Color.magenta;
    }

    public void DestroyBrick() {

        gameObject.SetActive( false ); //set false wait for destory

        //Call all variants BEFORE destroying this brick
        foreach ( var variant in GetComponents<IBrickVariant>() ) {
            try {
                variant.OnDie(this);
            }
            catch ( Exception e ) {
                Debug.LogException(e);
            }
        }

        Destroy(gameObject);
    }


    private void OnTriggerEnter2D( Collider2D collision ) {
        if ( collision.CompareTag("EndLine") ) {
            Debug.Log("Brick hit the end line!");
        }
    }


}
