using UnityEngine;

public class BrickScript : MonoBehaviour
{
    [SerializeField] private int health = 1;
    public void TakeDamage(int damage)
    {
        Debug.Log($"Taking damage: {damage}");
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }


    }
}
