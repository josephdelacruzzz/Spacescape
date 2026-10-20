using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState { Playing, GameOver, Won }

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Playing;
    public bool IsPlaying => State == GameState.Playing;

    public PlayerController player;
    public Transform mothership;
    public TMP_Text scoreText;
    public int Score => heightScore + killScore;
    private float startY;
    private int heightScore;
    private int killScore;
    private float bestHeight;

    void Awake() {
        Instance = this;
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
        if (!IsPlaying && Input.GetKeyDown(KeyCode.R))
            Restart();
    }

    public void AddKillScore(int points) {
        if (!IsPlaying)
            return;

        killScore += points;
        UpdateUI();
    }

    private void UpdateUI() {
        if (scoreText != null)
            scoreText.text = "Score: " + Score;
    }

    public void GameOver() {
        if (!IsPlaying)
            return;

        State = GameState.GameOver;
        UpdateUI();

        Debug.Log("GAME OVER. Score: " + Score);
    }
    
    private void Restart() {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }
}
