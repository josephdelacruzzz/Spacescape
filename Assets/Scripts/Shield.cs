using UnityEngine;
using TMPro;

public class Shield : MonoBehaviour {

    public float shieldDuration = 8f;
    public GameObject shieldEffect;
    public TMP_Text shieldText;
    public bool isActive { get; private set; }
    private bool shieldReady;
    private float timeLeft;


    void Start() {
        if (shieldEffect != null)
            shieldEffect.SetActive(false);

        UpdateUI();
    }

    void Update() {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
            return;

        if (!isActive && shieldReady && Input.GetKeyDown(KeyCode.E))
            ActivateShield();
        
        if (isActive) {
            timeLeft -= Time.deltaTime;
            UpdateUI();
            if (timeLeft <= 0f)
                DeactivateShield();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("ShieldPickup"))
            return;

        shieldReady = true;
        Destroy(other.gameObject);
        UpdateUI();

        Debug.Log("Shield collected. Press E to activate.");
    }

    private void ActivateShield() {
        shieldReady = false;
        isActive = true;
        timeLeft = shieldDuration;

        if (shieldEffect != null)
            shieldEffect.SetActive(true);

        UpdateUI();
        Debug.Log("Shield activated.");
    }

    private void DeactivateShield() {
        isActive = false;

        if (shieldEffect != null)
            shieldEffect.SetActive(false);

        UpdateUI();
        Debug.Log("Shield expired.");
    }

    public void OnShieldHit() {
        if (!isActive)
            return;

        Debug.Log("Shield blocked asteroid.");
    }

    private void UpdateUI() {
        if (shieldText == null)
            return;

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
