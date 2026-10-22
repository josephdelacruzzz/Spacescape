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

    public float crossScreenChance = 0.25f;
    public float edgeBand = 2f;
    public int crossScreenCooldown = 3;

    public float movingPlatformChance = 0.15f;
    public float movingPlatformSpeed = 1.5f;
    public float startingMovingSpeed = 1.5f;
    public float endingMovingSpeed = 3.5f;
    public float movingPlatformRange = 2f;

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

        bool prepareCrossScreen = false;
        int crossScreenCooldownRemaining = 0;


        for (int i = 0; i < platformCount; i++) {
            float t = i / (float)(platformCount - 1); 
            y += Mathf.Lerp(minGap, maxGap, t);
            float width = Mathf.Lerp(startWidth, endWidth, t);

            float limit = halfWidth - width / 2f;
            float x;

            bool crossScreenJump = prepareCrossScreen && i > 2;

            if (crossScreenJump){
                float band = Mathf.Min(edgeBand, limit);

                if (prevX >= 0f)
                    x = Random.Range(-limit, -limit + band);
                else
                    x = Random.Range(limit - band, limit);

                prepareCrossScreen = false;
                crossScreenCooldownRemaining = crossScreenCooldown;
            } else {
                bool startCrossScreenRoute = i > 2 && !prepareCrossScreen && crossScreenCooldownRemaining <= 0 && Random.value < crossScreenChance;

                if (startCrossScreenRoute) {
                    float edge = Random.value < 0.5f
                        ? -limit
                        : limit;

                    x = Mathf.Clamp(edge, prevX - horizontalStep, prevX + horizontalStep);

                    x = Mathf.Clamp(x, -limit, limit);
                    prepareCrossScreen = true;
                } else {
                    x = Random.Range(-limit, limit);
                    x = Mathf.Clamp(x, prevX - horizontalStep, prevX + horizontalStep);
                    x = Mathf.Clamp(x, -limit, limit);

                    if (crossScreenCooldownRemaining > 0)
                        crossScreenCooldownRemaining--;
                }
            }

            prevX = x;

            GameObject p = Instantiate(platform, new Vector3(x, y, 0f), Quaternion.identity, parent);
            p.transform.localScale = new Vector3(width, 0.4f, 1f);
            bool isMoving = i > 3 && Random.value < movingPlatformChance;
            if (isMoving){
                MovingLevels movingScript = p.GetComponent<MovingLevels>();
                if (movingScript == null) {
                    movingScript =
                    p.AddComponent<MovingLevels>();
                }
                if (movingScript != null) {
                    float currentMovmentSpeed = Mathf.Lerp(startingMovingSpeed, endingMovingSpeed, t);
                    movingScript.setMovement(currentMovmentSpeed, Mathf.Max(-limit, x-movingPlatformRange), Mathf.Min(limit, x+movingPlatformRange));
                }
            }

            if (ShieldPickup != null && !isMoving && i > 0 && i % pickupEveryNPlatforms == 0) {
                Instantiate(ShieldPickup, new Vector3(x, y + 0.8f, 0f), Quaternion.identity, parent);
            }
        }

        mothership.position = new Vector3(0f, y + 4f, 0f);
        Random.state = oldState; 
    }
}