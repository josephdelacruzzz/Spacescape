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

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update() {
        float h = Input.GetAxisRaw("Horizontal"); 
        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);
        CheckGround();
        HandleJump();
        WrapAroundScreen();
    }

    private void CheckGround() {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer)
                     && rb.linearVelocity.y <= 0.1f;
    }

    private void HandleJump() {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
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
}