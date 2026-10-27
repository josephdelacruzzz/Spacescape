using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {
    public GameObject asteroidPrefab;
    public float firstSpawnDelay = 2.25f; //delay before first asteroid appears
    //asteroid spawn interval 
    public float startInterval = 1.5f; 
    public float endInterval = 0.5f;
    //asteroid falling speed
    public float startSpeed = 2.5f;
    public float endSpeed = 7f;

    private Camera cam;
    private float timer;

    void Start() {
        cam = Camera.main;
        timer = firstSpawnDelay;
    }

    void Update() {
        //pauses asteroid spawning when panel is active
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) 
            return;
        if (asteroidPrefab == null || cam == null)
            return;

        //timer for interval of spawning asteroids
        timer -= Time.deltaTime;
        if (timer <= 0f) {
            SpawnAsteroid();
            timer = Mathf.Lerp(startInterval, endInterval, GameManager.Instance.Progress);
        }
    }

    private void SpawnAsteroid() {
        float halfWidth = cam.orthographicSize * cam.aspect; //calculate horizontal halfwidth of camera
        float x = Random.Range( -halfWidth + 0.5f, halfWidth - 0.5f); //random posotion within camera view
        float y = cam.transform.position.y + cam.orthographicSize + 2f; //spawnpoint above visible screen
        GameObject asteroidObject = Instantiate(asteroidPrefab, new Vector3(x, y, 0f), Quaternion.identity ); //create asteroid at position
        int sizeLevel = Random.Range(1, 4); //size of asteroid

        Asteroid asteroid = asteroidObject.GetComponent<Asteroid>();

        //increase falling speed based on progress,  size randomly chosen
        if (asteroid != null) {
            float currentSpeed = Mathf.Lerp(startSpeed, endSpeed, GameManager.Instance.Progress );
            asteroid.Init(sizeLevel,currentSpeed);
        }
    }
}