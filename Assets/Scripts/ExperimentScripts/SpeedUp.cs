using UnityEngine;

public class SpeedUp : MonoBehaviour {

    Rigidbody2D rb;

    public void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();

        rb.AddForce(5 * Vector2.up);
    }

}
