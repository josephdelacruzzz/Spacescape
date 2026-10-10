using UnityEngine;

public class Asteroid : MonoBehaviour {

    private int size = 1;
    private Rigidbody2D rb;
    private Camera cam;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    public void Init(int sizeLevel, float fallSpeed) {
        size = sizeLevel;
        transform.localScale = Vector3.one * (0.5f + 0.4f * sizeLevel); 
        rb.linearVelocity = new Vector2(0f, -fallSpeed);
    }

    void Update() {
        if (transform.position.y < cam.transform.position.y - cam.orthographicSize - 3f)
            Destroy(gameObject);
    }
}