using UnityEngine;

public class Shooting : MonoBehaviour {
    public GameObject projectilePrefab; 
    public Transform gun; //object rotating toward mouse
    public Transform fire; //points where projectiles are spawned
    public float fireCooldown = 0.5f; 
    private Camera cam;
    private float nextFireTime; //earliest time player can fire again
    private OneShot oneShot;

    void Start(){
        cam = Camera.main;
        oneShot = GetComponent<OneShot>();
    }

    void LateUpdate() {
        if (cam == null)
            return;

        //deactivates aim and fire while game is on a panel
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
            return;

        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition); //converts mouse screen pos to world pos
        Vector2 aimDirection = mouseWorldPosition - transform.position; //find dir from the player to the mouse
        if (aimDirection.sqrMagnitude < 0.01f)
            return;

        //convert dir to unit vector and roatte gun 
        aimDirection.Normalize();
        gun.right = aimDirection;

        //fire continuously with respect of cooldown
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime) {
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
        if (projectile != null)
            projectile.Launch(direction);
    }

}
