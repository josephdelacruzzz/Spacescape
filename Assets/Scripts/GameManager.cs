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
    public TMP_Text shieldText;
    public int Score => heightScore + killScore;
    private float startY;
    private int heightScore;
    private int killScore;
    private float bestHeight;
    public float Progress { get; private set; }
    private float goalHeight;

    public GameObject winPanel;
    public TMP_Text winScoreText;
    public TMP_Text winTitleText;

    public GameObject gameOverPanel;
    public TMP_Text gameOverScoreText;
    public TMP_Text gameOverTitleText;

    void Awake() {
        Instance = this;
        State = GameState.Playing;
    }

    void Start() {
        startY = player.transform.position.y;
        if (mothership != null) 
            goalHeight = mothership.position.y - startY;
        if (winPanel != null) 
            winPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update() {
        if (IsPlaying) {
            bestHeight = Mathf.Max(bestHeight, player.transform.position.y - startY);
            heightScore = Mathf.RoundToInt(bestHeight * 10f);
            if (goalHeight > 0f) 
                Progress = Mathf.Clamp01((player.transform.position.y - startY) / goalHeight);
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

    private void HideGameplayUI() {
        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

        if (shieldText != null)
            shieldText.gameObject.SetActive(false);
    }

    public void GameOver() {
        if (!IsPlaying)
            return;

        State = GameState.GameOver;
        HideGameplayUI();
        UpdateUI();

        if (gameOverTitleText != null)
            gameOverTitleText.text = "GAME OVER!";

        if (gameOverScoreText != null)
            gameOverScoreText.text = "Score: " + Score;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("GAME OVER. Score: " + Score);
    }
    
    private void Restart() {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }

    public void Win() {
        if (!IsPlaying) 
            return;

        State = GameState.Won;
        HideGameplayUI();
        UpdateUI();

        if (winTitleText != null)
                winTitleText.text = "YOU WIN!";
        if (winScoreText != null) 
            winScoreText.text = "Score: " + Score;
        if (winPanel != null) 
            winPanel.SetActive(true);

         Debug.Log("YOU WIN! Score: " + Score);
    }
}
