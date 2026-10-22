using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 6f; //horizonatl movement speed
    public float jumpSpeed = 12f;

    public Transform groundCheck; //check if player is on platform
    public float groundCheckRadius = 0.15f; //radius of ground check circle
    public LayerMask groundLayer; //ground or platfroms

    private Rigidbody2D rb;
    private Camera cam;
    private bool isGrounded; //current and prev grounded states
    private bool hasBeenHit; //prevents multiple death or win evens
    public float bounceMultiplier = 3f;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update() {
        //stops player controls if game not active
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) 
            return;
        float h = Input.GetAxisRaw("Horizontal"); //A or D for left right input
        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);
        CheckGround(); //check if player on platform
        HandleJump(); //single jump
        WrapAroundScreen(); //moe player to opposite side if leave left or right
        CheckFellOffScreen(); //gameover if player falls below camerea
    }

    private void CheckGround() {
        // wasGrounded = isGrounded; //save prev grounded state
        //player is grounded when check circle overlaps fround layer obj and player is not moving up
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer)
                     && rb.linearVelocity.y <= 0.1f;
    }

    private void HandleJump() {
        //jump performed when Space pressed and on ground
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
    }

    private void WrapAroundScreen() {
        float limit = cam.orthographicSize * cam.aspect + 0.5f; //calculate cam's horiz boundary
        Vector3 p = transform.position;
        if (p.x > limit) { //move player to opposite side of screen
            p.x = -limit;
        } else if (p.x < -limit) {
            p.x = limit;
        }
        transform.position = p;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (hasBeenHit) //ignore collisions if player won or lost
            return;

        if (other.CompareTag("Goal")) { //reaching motership ends level
            hasBeenHit = true;
            if (GameManager.Instance != null) 
                GameManager.Instance.Win();
            if (rb != null)  //stops movement
                rb.linearVelocity = Vector2.zero;
            gameObject.SetActive(false); //hides player after reaching goal
            return;
        }

        //check if trigger is beacuse of shield or asteroid
        Asteroid asteroid = other.GetComponent<Asteroid>();
        Shield shield = GetComponent<Shield>();
        if (asteroid == null)
            return;

        //active shield destroys asteroid instead of player
        if (shield != null && shield.isActive) {
            shield.OnShieldHit();
            asteroid.Break();
            return;
        }
            
        Debug.Log("Player hit by asteroid.");

        //asteroid kills player if shield inactive
        Destroy(asteroid.gameObject);
        Die(); 
    }

    void OnCollisionEnter2D(Collision2D collision) {
        //ignore collisions if not a spring object
        if (collision.contactCount == 0 || !collision.gameObject.CompareTag("Spring"))
            return;
        if (collision.GetContact(0).normal.y > 0.5f) { //bounces when landing on top of spring
            float bounceSpeed = jumpSpeed * Mathf.Sqrt(bounceMultiplier);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceSpeed);
        }
    }

    private void Die() {
        if (hasBeenHit) //allows only one death
            return;

        hasBeenHit = true;

        //shows the gameover state
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
        

        gameObject.SetActive(false); //hides the player after death
    }

    private void CheckFellOffScreen() {
        if (cam == null || hasBeenHit) //if there is no camera or if the player already died
            return;

        //calculate point below visible camera screen
        float bottomLimit = cam.transform.position.y - cam.orthographicSize - 1f; 

        //gameover if player falls below point
        if (transform.position.y < bottomLimit) {
            Debug.Log("Player fell off the screen.");
            Die();
        }
    }
}