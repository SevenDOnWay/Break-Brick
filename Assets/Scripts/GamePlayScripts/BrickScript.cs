using System;
using UnityEngine;

public class BrickScript : MonoBehaviour
{
    [SerializeField] private int health = 1;

    public static event EventHandler OnBrickDestroyed;
    public static event EventHandler OnBrickHit;


    public void TakeDamage(int damage)
    {
        Debug.Log($"Taking damage: {damage}");
        health -= damage;

        if (health <= 0)
        {
            Debug.Log("Brick destroyed");
            OnBrickDestroyed?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
            
        }
        else
        {
            OnBrickHit?.Invoke(this, EventArgs.Empty);
        }

        Debug.Log($"Brick health remaining: {health}");

    }
}
