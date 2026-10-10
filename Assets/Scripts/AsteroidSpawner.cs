using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {
    public GameObject asteroidPrefab;
    public float spawnInterval = 2.5f;
    public float fallSpeed = 3f;

    private Camera cam;
    private float timer;

    void Start() {
        cam = Camera.main;
        timer = spawnInterval;
    }

    void Update() {
        if (asteroidPrefab == null || cam == null)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f) {
            SpawnAsteroid();
            timer = spawnInterval;
        }
    }

    private void SpawnAsteroid() {
        float halfWidth = cam.orthographicSize * cam.aspect;
        float x = Random.Range( -halfWidth + 0.5f, halfWidth - 0.5f);
        float y = cam.transform.position.y + cam.orthographicSize + 2f;
        GameObject asteroidObject = Instantiate(asteroidPrefab, new Vector3(x, y, 0f), Quaternion.identity );

        int sizeLevel = Random.Range(1, 4);

        Asteroid asteroid = asteroidObject.GetComponent<Asteroid>();

        if (asteroid != null) {
            asteroid.Init(sizeLevel,fallSpeed);
        }
    }
}