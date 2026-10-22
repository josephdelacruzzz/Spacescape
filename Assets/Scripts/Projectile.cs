using UnityEngine;

public class Projectile : MonoBehaviour{

    public float speed = 10f;
    public float lifetime = 2f; //amount of time projectile exists
    public int damage = 1;
    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction) {
        direction.Normalize(); //converts dir into unit vector so speed is const in every dir
        rb.linearVelocity = direction * speed; //move proj in aimed dir
        transform.right = direction; //rotate proj so its right side points in dir its travelling
        Destroy(gameObject, lifetime); //remove proj after timer up
    }

    void OnTriggerEnter2D(Collider2D other) {
        Asteroid asteroid = other.GetComponent<Asteroid>(); //check if proj hit asteroid
        if (asteroid == null)
            return;
        asteroid.TakeDamage(damage); //apply damage to asteroid
        Destroy(gameObject); //destory proj after it hits asteroid.
        
    }
}
