using System;
using TMPro;
using UnityEngine;
using VContainer;

public class BrickScript : MonoBehaviour {
    private int health;
    [Inject] LevelManager levelManager;

    [SerializeField] TextMeshPro healText;
    //TODO: Add color, and type variation to the brick

    public event Action<BrickScript> OnBrickDestroyed;

    public void Init( int health ) {
        this.health = health;
        healText.text = $"{health}";
    }

    public void TakeDamage( int damage ) {
        //Debug.Log($"Taking damage: {damage}");
        health -= damage;
        healText.text = health.ToString();
        levelManager.AddExp(damage);

        if ( health <= 0 ) {
            Destroy(gameObject);
        }


    }

    private void DestroyBrick() {
        OnBrickDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if ( collision.CompareTag("EndLine") ) {
            Debug.Log("Brick hit the end line!");
        }
    }

}
