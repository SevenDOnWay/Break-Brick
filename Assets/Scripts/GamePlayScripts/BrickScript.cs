using System;
using TMPro;
using UnityEngine;

public class BrickScript : MonoBehaviour {
    private int health;

    //private TextMeshPro  healText; // TODO: add number of health to the brick, and show it on the brick, somehow add this lead to error.

    public event Action<BrickScript> OnBrickDestroyed;


    //private void Start() {
    //    healText = gameObject.GetComponent<TextMeshPro>();
    //    if ( healText == null ) {
    //        Debug.LogError("HealText is not assigned in BrickScript.");
    //    }
    //}

    public void Init( int health ) {
        this.health = health;
            //healText.text = health.ToString();
    }

    public void TakeDamage( int damage ) {
        Debug.Log($"Taking damage: {damage}");
        health -= damage;
        //healText.text = health.ToString();
        if ( health <= 0 ) {
            Destroy(gameObject);
        }
        

    }

    private void DestroyBrick() {
        OnBrickDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

}
