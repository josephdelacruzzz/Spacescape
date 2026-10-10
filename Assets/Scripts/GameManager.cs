using UnityEngine;
using TMPro;

public enum GameState { Playing, GameOver, Won }

public class GameManager : MonoBehaviour {

    public GameState State { get; private set; } = GameState.Playing;
    public bool IsPlaying => State == GameState.Playing;

    public PlayerController player;
    public Transform mothership;
    public TMP_Text scoreText;
    public int Score => heightScore;
    private float startY;
    private int heightScore;
    private float bestHeight;

    void Awake() {
        State = GameState.Playing;
    }

    void Start() {
        startY = player.transform.position.y;
    }

    void Update() {
        if (IsPlaying) {
            bestHeight = Mathf.Max(bestHeight, player.transform.position.y - startY);
            heightScore = Mathf.RoundToInt(bestHeight * 10f);
            UpdateUI();
        }
    }

    private void UpdateUI() {
        if (scoreText != null) {
            scoreText.text = "Score: " + Score;
        }
    }
}
