using UnityEngine;
using TMPro;

public class OneShot : MonoBehaviour {

    public float oneShotDuration = 8f;
    public TMP_Text oneShotText; //UI text display
    public bool isActive { get; private set; }
    private bool oneShotReady;
    private float timeLeft;


    void Start() {
        UpdateUI(); //sets initial text
    }

    void Update() {
        //input and timer deactivated if on panel
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
            return;

        //activates one shot
        if (!isActive && oneShotReady && Input.GetKeyDown(KeyCode.F))
            ActivateOneShot();
        
        //counts down duration
        if (isActive) {
            timeLeft -= Time.deltaTime;
            UpdateUI();
            if (timeLeft <= 0f)
                DeactivateOneShot();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("OneShotPickup")) //only accepts oneshot pickups
            return;
        oneShotReady = true;
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.oneShotPickupSound);
        Destroy(other.gameObject);
        UpdateUI();

        Debug.Log("OneShot collected. Press F to activate.");
    }

    private void ActivateOneShot() {
        //start active state and timer
        oneShotReady = false;
        isActive = true;
        timeLeft = oneShotDuration;
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.oneShotActivateSound);
        UpdateUI();
        Debug.Log("OneShot activated.");
    }

    private void DeactivateOneShot() {
        //oneshot deactivated after timer expires
        isActive = false;
        UpdateUI();
        Debug.Log("OneShot expired.");
    }

    private void UpdateUI() {
        if (oneShotText == null)
            return;

        //displays correct UI for oneshot
        if (isActive) {
            oneShotText.text = "OneShot: " + Mathf.CeilToInt(timeLeft) + "s";
        }
        else if (oneShotReady) {
            oneShotText.text = "OneShot: (F)";
        }
        else {
            oneShotText.text = "OneShot: --";
        }
    }

}
