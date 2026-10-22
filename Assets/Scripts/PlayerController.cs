using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 6f;
    public float jumpSpeed = 12f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Camera cam;
    private bool isGrounded, wasGrounded;
    private bool hasBeenHit;
    public float bounceMultiplier = 3f;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update() {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) 
            return;
        float h = Input.GetAxisRaw("Horizontal"); 
        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);
        CheckGround();
        HandleJump();
        WrapAroundScreen();
        CheckFellOffScreen();
    }

    private void CheckGround() {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer)
                     && rb.linearVelocity.y <= 0.1f;
    }

    private void HandleJump() {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        
    }

    private void WrapAroundScreen() {
        float limit = cam.orthographicSize * cam.aspect + 0.5f;
        Vector3 p = transform.position;
        if (p.x > limit) {
            p.x = -limit;
        } else if (p.x < -limit) {
            p.x = limit;
        }
        transform.position = p;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (hasBeenHit)
            return;

        if (other.CompareTag("Goal")) {
            hasBeenHit = true;
            if (GameManager.Instance != null) 
                GameManager.Instance.Win();

            if (rb != null) 
                rb.linearVelocity = Vector2.zero;

            gameObject.SetActive(false);
            return;
        }

        Asteroid asteroid = other.GetComponent<Asteroid>();
        Shield shield = GetComponent<Shield>();
        if (asteroid == null)
            return;

        if (shield != null && shield.isActive) {
            shield.OnShieldHit();
            asteroid.Break();
            return;
        }
            
        Debug.Log("Player hit by asteroid.");

        Destroy(asteroid.gameObject);
        Die();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.contactCount == 0 || !collision.gameObject.CompareTag("Spring"))
            return;
        if (collision.GetContact(0).normal.y > 0.5f) {
            float bounceSpeed = jumpSpeed * Mathf.Sqrt(bounceMultiplier);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceSpeed);
        }
    }

    private void Die() {
        if (hasBeenHit)
            return;

        hasBeenHit = true;

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
        

        gameObject.SetActive(false);
    }

    private void CheckFellOffScreen() {
        if (cam == null || hasBeenHit)
            return;

        float bottomLimit = cam.transform.position.y - cam.orthographicSize - 1f;

        if (transform.position.y < bottomLimit) {
            Debug.Log("Player fell off the screen.");
            Die();
        }
    }
}