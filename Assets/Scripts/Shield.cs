using UnityEngine;
using TMPro;

public class Shield : MonoBehaviour {

    public float shieldDuration = 8f;
    public GameObject shieldEffect;
    public TMP_Text shieldText; //UI text display
    public bool isActive { get; private set; }
    private bool shieldReady;
    private float timeLeft;


    void Start() {
        if (shieldEffect != null)
            shieldEffect.SetActive(false);

        UpdateUI(); //sets initial text
    }

    void Update() {
        //input and timer deactivated if on panel
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
            return;

        //activates oneshot
        if (!isActive && shieldReady && Input.GetKeyDown(KeyCode.E))
            ActivateShield();
        
        //counts down duration
        if (isActive) {
            timeLeft -= Time.deltaTime;
            UpdateUI();
            if (timeLeft <= 0f)
                DeactivateShield();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("ShieldPickup"))//only accepts shield pickups
            return;
        shieldReady = true;
        Destroy(other.gameObject);
        UpdateUI();

        Debug.Log("Shield collected. Press E to activate.");
    }

    private void ActivateShield() {
        //start active state and timer
        shieldReady = false;
        isActive = true;
        timeLeft = shieldDuration;
        if (shieldEffect != null)
            shieldEffect.SetActive(true);
        UpdateUI();
        Debug.Log("Shield activated.");
    }

    private void DeactivateShield() {
        //shield deactivated after timer expires
        isActive = false;

        if (shieldEffect != null)
            shieldEffect.SetActive(false);

        UpdateUI();
        Debug.Log("Shield expired.");
    }

    public void OnShieldHit() {
        if (!isActive)
            return;
        //protects player from asteroid
        Debug.Log("Shield blocked asteroid.");
    }

    private void UpdateUI() {
        if (shieldText == null)
            return;

        //displays correct UI for shield
        if (isActive) {
            shieldText.text = "Shield: " + Mathf.CeilToInt(timeLeft) + "s";
        }
        else if (shieldReady) {
            shieldText.text = "Shield: Ready (E)";
        }
        else {
            shieldText.text = "Shield: --";
        }
    }

}
