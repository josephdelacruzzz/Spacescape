using UnityEngine;

public class Shooting : MonoBehaviour {

    public GameObject projectilePrefab;
    public Transform gun;
    public Transform fire;
    public float fireCooldown = 0.5f;
    private Camera cam;
    private float nextFireTime;
    private OneShot oneShot;

    void Start(){
        cam = Camera.main;
        oneShot = GetComponent<OneShot>();
    }

    void LateUpdate() {
        if (cam == null)
            return;

        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
            return;

        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = mouseWorldPosition - transform.position;
        if (aimDirection.sqrMagnitude < 0.01f)
            return;

        aimDirection.Normalize();
        gun.right = aimDirection;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime) {
            nextFireTime = Time.time + fireCooldown;
            Fire(aimDirection);
        }
    }

    private void Fire(Vector2 direction) {
        if (projectilePrefab == null || fire == null)
            return;

        GameObject projectileObject = Instantiate(projectilePrefab, fire.position, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (oneShot != null && oneShot.isActive)
            projectile.damage = 999;

        if (projectile != null)
            projectile.Launch(direction);
    }

}
