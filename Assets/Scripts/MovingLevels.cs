using UnityEngine;

public class MovingLevels : MonoBehaviour{

    private bool isMoving;
    private float moveSpeed;
    private float leftLimit;
    private float rightLimit;
    private int direction = 1;
    private Rigidbody2D rb;
    
    public void setMovement(float speed, float left, float right) {
        isMoving = true;
        moveSpeed = speed;
        leftLimit = left;
        rightLimit = right;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (Random.value < 0.5f)
            direction = -1;
    }

    void FixedUpdate() {
        if (!isMoving) 
            return;
        
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) 
            return;

        Vector2 position = rb.position;

        position.x += direction * moveSpeed * Time. fixedDeltaTime;

        if (position.x <= leftLimit) {
            position.x = leftLimit;
            direction = 1;
        } else if (position.x >= rightLimit) {
            position.x = rightLimit;
            direction = -1;
        }

        rb.MovePosition(position);
    }
}
