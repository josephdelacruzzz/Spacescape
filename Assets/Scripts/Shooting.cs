using UnityEngine;

public class Shooting : MonoBehaviour {
    public GameObject projectilePrefab; 
    public Transform gun; //object rotating toward mouse
    public Transform fire; //points where projectiles are spawned
    public float fireCooldown = 0.5f; 
    private Camera cam;
    private float nextFireTime; //earliest time player can fire again
    private OneShot oneShot;
    public float minAimAngle = 20f;
    public float maxAimAngle = 160f;

    private Animator animator;
    private SpriteRenderer bodyRenderer;
    private bool wasShooting;
    private static int IsShootingHash = Animator.StringToHash("IsShooting");
    private static int AimUpHash = Animator.StringToHash("AimUp");

    void Start(){
        cam = Camera.main;
        oneShot = GetComponent<OneShot>();
        animator = GetComponent<Animator>();
        bodyRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate() {
        if (cam == null)
            return;
        
        bool IsShooting = Input.GetMouseButton(0);

        //deactivates aim and fire while game is on a panel
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) {
            if (animator != null)
                animator.SetBool(IsShootingHash, false);
            return;
        }

        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition); //converts mouse screen pos to world pos
        Vector2 aimDirection = mouseWorldPosition - transform.position; //find dir from the player to the mouse
        if (aimDirection.sqrMagnitude < 0.01f)
            return;

        //convert dir to unit vector and roatte gun 
        aimDirection.Normalize();
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        if (angle < 0f)
            angle += 360f;
        angle = Mathf.Clamp(angle, minAimAngle, maxAimAngle);
        aimDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        gun.right = aimDirection;
        if (Mathf.Abs(aimDirection.x) > 0.1f && bodyRenderer != null) 
            bodyRenderer.flipX = aimDirection.x < 0f;
        bool aimingUp = angle >= 67.5f && angle <= 112.5f;
        if (animator != null) {
            animator.SetBool(IsShootingHash, IsShooting);
            animator.SetBool(AimUpHash, aimingUp);
        }
        if (IsShooting && Time.time >= nextFireTime) {
            nextFireTime = Time.time + fireCooldown;
            Fire(aimDirection);
        }
    }

    private void Fire(Vector2 direction) {
        if (projectilePrefab == null || fire == null)
            return;

        //creates projectile at firing point
        GameObject projectileObject = Instantiate(projectilePrefab, fire.position, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        //makes projectile oneshot if powerup is active
        if (oneShot != null && oneShot.isActive)
            projectile.damage = 999;
        //launches projectile in dir of mouse
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.shootSound);
        projectile.Launch(direction);
    }

}

