using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public float yOffset = 10f; //distance between player and camera pos
    public float followSpeed = 5f; //how fast camera moves toward height
    private float baseY; //curr pos of camera
    private float highestTargetY; //highest pos reached

    void Start() {
        baseY = transform.position.y; //starting height
        highestTargetY = baseY; //camera doesnt begin below starting height
    }

    void LateUpdate() {
        //camera follows the players position smoothly
        if (target != null) {
            highestTargetY = Mathf.Max(highestTargetY, target.position.y + yOffset);
            baseY = Mathf.Lerp(baseY, highestTargetY, followSpeed * Time.deltaTime);
        }
        //keeps camera in the center and constant
        transform.position = new Vector3(0f, baseY, -10f);
    }
}