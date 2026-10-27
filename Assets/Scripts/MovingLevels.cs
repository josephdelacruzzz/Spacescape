using UnityEngine;

public class MovingLevels : MonoBehaviour{
    private bool isMoving;
    private float moveSpeed;
    //left and right movement boundaries
    private float leftLimit;
    private float rightLimit;
    private int direction = 1; //movement direction (1=right,-1=left)
    private Rigidbody2D rb;
    
    //called by Levels when platform should move
    public void setMovement(float speed, float left, float right) {
        isMoving = true;
        moveSpeed = speed;
        leftLimit = left;
        rightLimit = right;
        rb = GetComponent<Rigidbody2D>();

        //add rigidbody if it does not exist
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        //sets correct settings for platform to move
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        //randomly choose direction of movement
        if (Random.value < 0.5f)
            direction = -1;
    }

    void FixedUpdate() {
        if (!isMoving) 
            return;
        
        //stop movement if a panel is active
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) 
            return;
        Vector2 position = rb.position;
        position.x += direction * moveSpeed * Time. fixedDeltaTime; //move horizontally
        //reverses direction when platform reaches boundary
        if (position.x <= leftLimit) {
            position.x = leftLimit;
            direction = 1;
        } else if (position.x >= rightLimit) {
            position.x = rightLimit;
            direction = -1;
        }
        //move rigidbody to calculated position
        rb.MovePosition(position);
    }
}
