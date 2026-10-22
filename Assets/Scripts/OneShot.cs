using UnityEngine;
using TMPro;

public class OneShot : MonoBehaviour {

    public float oneShotDuration = 8f;
    public TMP_Text oneShotText;
    public bool isActive { get; private set; }
    private bool oneShotReady;
    private float timeLeft;


    void Start() {
        UpdateUI();
    }

    void Update() {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
            return;

        if (!isActive && oneShotReady && Input.GetKeyDown(KeyCode.F))
            ActivateOneShot();
        
        if (isActive) {
            timeLeft -= Time.deltaTime;
            UpdateUI();
            if (timeLeft <= 0f)
                DeactivateOneShot();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("OneShotPickup"))
            return;

        oneShotReady = true;
        Destroy(other.gameObject);
        UpdateUI();

        Debug.Log("OneShot collected. Press F to activate.");
    }

    private void ActivateOneShot() {
        oneShotReady = false;
        isActive = true;
        timeLeft = oneShotDuration;

        UpdateUI();
        Debug.Log("OneShot activated.");
    }

    private void DeactivateOneShot() {
        isActive = false;

        UpdateUI();
        Debug.Log("OneShot expired.");
    }

    private void UpdateUI() {
        if (oneShotText == null)
            return;

        if (isActive) {
            oneShotText.text = "OneShot: " + Mathf.CeilToInt(timeLeft) + "s";
        }
        else if (oneShotReady) {
            oneShotText.text = "OneShot: Ready (F)";
        }
        else {
            oneShotText.text = "OneShot: --";
        }
    }

}
