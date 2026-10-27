using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; } //access AudioManager

    //audio clips in the game
    public AudioClip jumpSound;
    public AudioClip springSound;
    public AudioClip shootSound;
    public AudioClip asteroidDestroySound;
    public AudioClip playerHitSound;
    public AudioClip shieldPickupSound;
    public AudioClip shieldActivateSound;
    public AudioClip shieldBlockSound;
    public AudioClip oneShotPickupSound;
    public AudioClip oneShotActivateSound;
    public AudioClip startSound;
    public AudioClip winSound;
    public AudioClip gameOverSound;

    private AudioSource audioSource;

    void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip) {
        Debug.Log("Playing sound: " + clip.name);
        audioSource.PlayOneShot(clip); //plays sound effect
    }
}