using UnityEngine;

public class Levels : MonoBehaviour {
    public GameObject platform;
    public Transform mothership;

    public int platformCount = 45;
    public float firstGroundY = -5f;
    public float minGap = 2.1f;
    public float maxGap = 2.3f;
    public float startWidth = 3f;
    public float endWidth = 1.8f;
    public float horizontalStep = 4f; 
    public GameObject ShieldPickup;
    public int pickupEveryNPlatforms = 12;
    //public int seed = 1;

    void Awake() {
        Build();
    }

    private void Build() {
        Random.State oldState = Random.state;
        Random.InitState(System.Environment.TickCount);

        float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        Transform parent = new GameObject("Platform").transform;

        float y = firstGroundY;
        float prevX = 0f;

        for (int i = 0; i < platformCount; i++) {
            float t = i / (float)(platformCount - 1); 
            y += Mathf.Lerp(minGap, maxGap, t);
            float width = Mathf.Lerp(startWidth, endWidth, t);

            float limit = halfWidth - width / 2f; 
            float x = Random.Range(-limit, limit);
            x = Mathf.Clamp(x, prevX - horizontalStep, prevX + horizontalStep);
            x = Mathf.Clamp(x, -limit, limit);
            prevX = x;

            GameObject p = Instantiate(platform, new Vector3(x, y, 0f), Quaternion.identity, parent);
            p.transform.localScale = new Vector3(width, 0.4f, 1f);

            if (ShieldPickup != null && i > 0 && i % pickupEveryNPlatforms == 0) 
                Instantiate(ShieldPickup, new Vector3(x, y + 0.8f, 0f), Quaternion.identity, parent);
        }

        mothership.position = new Vector3(0f, y + 4f, 0f);
        Random.state = oldState; 
    }
}