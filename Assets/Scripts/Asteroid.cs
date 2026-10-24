using UnityEngine;

public class Asteroid : MonoBehaviour {

    private int size = 1;
    public int pointsPerSize = 10;
    private int asteroidHealth = 1;
    private Rigidbody2D rb;
    private Camera cam;
    public float rotationSpeed = 90f;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    //called by AsteroidSpawner after asteroid is instantiated
    public void Init(int sizeLevel, float fallSpeed) {
        size = sizeLevel;
        asteroidHealth = sizeLevel;
        transform.localScale = Vector3.one * (0.5f + 0.4f * sizeLevel);
        rb.linearVelocity = new Vector2(0f, -fallSpeed);
        rb.angularVelocity = Random.value < 0.5f ? rotationSpeed : -rotationSpeed;
    }

    //projectile hits asteroid
    public void TakeDamage(int amount) {
        asteroidHealth -= amount;

        if (asteroidHealth <= 0)
            Break();
    }

    //removes asteroid and gives points to player
    public void Break() {
        if (GameManager.Instance != null)
            GameManager.Instance.AddKillScore(pointsPerSize * size );

        Destroy(gameObject);
    }

    //destroys asteroids if not in camera view to prevent buildup of asteroids
    void Update() {
        if (transform.position.y < cam.transform.position.y - cam.orthographicSize - 3f)
            Destroy(gameObject);
    }
}