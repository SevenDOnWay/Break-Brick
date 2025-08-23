using System;
using TMPro;
using UnityEngine;

public class BrickScript : MonoBehaviour {
    private int health;

    [SerializeField] TextMeshPro healText;
    //TODO: Add color variation to the brick

    public event Action<BrickScript> OnBrickDestroyed;

    public void Init( int health ) {
        this.health = health;
        healText.text = $"{health}";
    }

    public void TakeDamage( int damage ) {
        Debug.Log($"Taking damage: {damage}");
        health -= damage;
        healText.text = health.ToString();

        if ( health <= 0 ) {
            Destroy(gameObject);
        }


    }

    private void DestroyBrick() {
        OnBrickDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

}
