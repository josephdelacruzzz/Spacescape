using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public float yOffset = 10f;     
    public float followSpeed = 5f;

    private float baseY;
    private float highestTargetY;

    void Start() {
        baseY = transform.position.y;
        highestTargetY = baseY;
    }

    void LateUpdate() {
        if (target != null) {
            highestTargetY = Mathf.Max(highestTargetY, target.position.y + yOffset);
            baseY = Mathf.Lerp(baseY, highestTargetY, followSpeed * Time.deltaTime);
        }

        transform.position = new Vector3(0f, baseY, -10f);
    }
}