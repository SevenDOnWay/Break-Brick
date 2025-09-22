using System;
using TMPro;
using UnityEngine;
using VContainer;

public class BrickScript : MonoBehaviour {
    [HideInInspector] public int health;
    [Inject] LevelManager levelManager;

    [SerializeField] TextMeshPro healText;

    public static event EventHandler OnBrickDestroyed;
    public static event EventHandler OnBrickHit;

    public void Init( int health ) {
        this.health = health;

        foreach ( var variant in GetComponents<IBrickVariant>() ) {
            try {
                variant.OnSpawn(this);
            }
            catch ( Exception e ) {
                Debug.LogException(e);
            }
        }

        UpdateHealthText();
    }

    public void UpdateHealthText() {
        healText.text = health.ToString();
    }

    public void TakeDamage( int damage ) {
        //Debug.Log($"Taking damage: {damage}");
        health -= damage;
        UpdateHealthText();
        levelManager.AddExp(damage);
            
        if ( health <= 0 ) {
            DestroyBrick();
        }
    }

    void DestroyBrick() {
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
