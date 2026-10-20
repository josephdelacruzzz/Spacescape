using UnityEngine;

public class Projectile : MonoBehaviour{

    public float speed = 10f;
    public float lifetime = 2f;
    public int damage = 1;
    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction) {
        direction.Normalize();
        rb.linearVelocity = direction * speed;
        transform.right = direction;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other) {
        Asteroid asteroid = other.GetComponent<Asteroid>();
        if (asteroid == null)
            return;
        asteroid.TakeDamage(damage);
        Destroy(gameObject);
        
    }
}
