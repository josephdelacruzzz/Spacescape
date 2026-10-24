using UnityEngine;

public class Levels : MonoBehaviour {
    public GameObject platform;
    public Transform mothership;

    [Header("Platform Layout")]
    public int platformCount = 50;
    public float firstGroundY = -5f;
    //distance between platforms
    public float minGap = 2.1f;
    public float maxGap = 2.3f;
    public float startWidth = 3f;
    public float endWidth = 1.8f;
    public float horizontalStep = 4f; 

    [Header("Powerups")]
    public GameObject ShieldPickup;
    public float shieldChance = 0.1f;
    public GameObject OneShotPickup;
    public float oneShotChance = 0.1f;

    //public int seed = 1;

    [Header("Cross-Screen")]
    public float crossScreenChance = 0.25f;
    public float edgeBand = 2f;
    public int crossScreenCooldown = 3;

    [Header("Moving Platforms")]
    public float movingPlatformChance = 0.15f;
    public float movingPlatformSpeed = 1.5f;
    public float startingMovingSpeed = 1.5f;
    public float endingMovingSpeed = 3.5f;
    public float movingPlatformRange = 2f;

    [Header("Spring")]
    public GameObject springPrefab;
    public float springChance = 0.12f;

    void Awake() {
        Build();
    }

    private void Build() {
        Random.State oldState = Random.state; //random level generation
        Random.InitState(System.Environment.TickCount); //different seeds

        float halfWidth = Camera.main.orthographicSize * Camera.main.aspect; //horizontal half width
        Transform parent = new GameObject("Platform").transform; //keeps generated object organized

        float y = firstGroundY;
        float prevX = 0f;

        //occassional jump from one side to opposite side
        bool prepareCrossScreen = false;
        int crossScreenCooldownRemaining = 0;


        for (int i = 0; i < platformCount; i++) {
            float t = i / (float)(platformCount - 1);  //progress through th elevel
            y += Mathf.Lerp(minGap, maxGap, t); //increases vertical jump
            float width = Mathf.Lerp(startWidth, endWidth, t); //makes platforms narrower
            float limit = halfWidth - width / 2f; //makes platforms stay within cam's view
            float x;
            bool crossScreenJump = prepareCrossScreen && i > 2; //cross screen route

            if (crossScreenJump){ //places platform near opposite edge
                float band = Mathf.Min(edgeBand, limit);

                if (prevX >= 0f)
                    x = Random.Range(-limit, -limit + band);
                else
                    x = Random.Range(limit - band, limit);

                prepareCrossScreen = false;
                crossScreenCooldownRemaining = crossScreenCooldown;
            } else { 
                //decide if we begin a cross screen route
                bool startCrossScreenRoute = i > 2 && !prepareCrossScreen && crossScreenCooldownRemaining <= 0 && Random.value < crossScreenChance;

                if (startCrossScreenRoute) { //place current platform near one edge
                    float edge = Random.value < 0.5f ? -limit : limit;
                    x = Mathf.Clamp(edge, prevX - horizontalStep, prevX + horizontalStep);
                    x = Mathf.Clamp(x, -limit, limit);
                    prepareCrossScreen = true;
                } else { //generate normal platform enar prev platform
                    x = Random.Range(-limit, limit);
                    x = Mathf.Clamp(x, prevX - horizontalStep, prevX + horizontalStep);
                    x = Mathf.Clamp(x, -limit, limit);
                    if (crossScreenCooldownRemaining > 0)
                        crossScreenCooldownRemaining--;
                }
            }

            prevX = x; //store curr platform pos for next platform

            //creates and resizes platform
            GameObject p = Instantiate(platform, new Vector3(x, y, 0f), Quaternion.identity, parent);
            p.transform.localScale = new Vector3(width, width, 1f);
            BoxCollider2D platformCollider = p.GetComponent<BoxCollider2D>();
            if (platformCollider != null)
                platformCollider.size = new Vector2(0.6f, 0.4f / width);
            bool isMoving = i > 3 && Random.value < movingPlatformChance; //some platforms begin moving
            if (isMoving){
                MovingLevels movingScript = p.GetComponent<MovingLevels>();
                if (movingScript == null) {
                    movingScript = p.AddComponent<MovingLevels>();
                }
                if (movingScript != null) { //increases speed of platforms further up
                    float currentMovmentSpeed = Mathf.Lerp(startingMovingSpeed, endingMovingSpeed, t);
                    movingScript.setMovement(currentMovmentSpeed, Mathf.Max(-limit, x-movingPlatformRange), Mathf.Min(limit, x+movingPlatformRange));
                }
            }

            //only one special object per platform
            if (ShieldPickup != null && !isMoving && i > 0 && Random.value < shieldChance) {
                Instantiate(ShieldPickup, new Vector3(x, y + 0.8f, 0f), Quaternion.identity, parent);
            } else if (springPrefab != null && !isMoving && i > 2 && Random.value < springChance) {
                Instantiate( springPrefab, new Vector3(x, y + 0.3f, 0f), Quaternion.identity, parent);
            } else if (OneShotPickup != null && !isMoving && i > 2 && Random.value < oneShotChance){ 
                Instantiate( OneShotPickup, new Vector3(x, y + 0.8f, 0f), Quaternion.identity, parent);
            }
        }

        mothership.position = new Vector3(0f, y + 4f, 0f); //place mothership above final platform
        Random.state = oldState; 
    }
}