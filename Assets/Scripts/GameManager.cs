using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState { Title, Playing, GameOver, Won }

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Playing;
    public bool IsPlaying => State == GameState.Playing;

    public PlayerController player;
    public Transform mothership;
    public TMP_Text scoreText;
    public TMP_Text shieldText;
    public TMP_Text oneShotText;
    public int Score => heightScore + killScore + timeBonus;
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

    public GameObject titlePanel;
    private static bool restartDirectly;

    public TMP_Text timerText;
    public int startingTime = 999;
    public float timerSpeed = 1f;

    private float timeRemaining;
    private int timeBonus;

    void Awake() {
        Instance = this;
        if (restartDirectly) {
            State = GameState.Playing;
            restartDirectly = false;
        } else {
            State = GameState.Title;
        }
    }

    void Start() {
        timeRemaining = startingTime;
        startY = player.transform.position.y;
        if (mothership != null) 
            goalHeight = mothership.position.y - startY;
        if (winPanel != null) 
            winPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (State == GameState.Title) {
            if (titlePanel != null) 
                titlePanel.SetActive(true);
            HideGameplayUI();
        } else {
            if (titlePanel != null)
                titlePanel.SetActive(false);
            if (scoreText != null)
                scoreText.gameObject.SetActive(true);
            if (shieldText != null)
                shieldText.gameObject.SetActive(true);
            if (oneShotText != null)
                oneShotText.gameObject.SetActive(true);
        }
    }

    public void StartGame() {
        if (State != GameState.Title)
            return;

        State = GameState.Playing;

        if (titlePanel != null)
            titlePanel.SetActive(false);
        if (scoreText != null)
            scoreText.gameObject.SetActive(true);
        if (shieldText != null)
            shieldText.gameObject.SetActive(true);
        if (oneShotText != null)
            oneShotText.gameObject.SetActive(true);
        if (timerText != null)
            timerText.gameObject.SetActive(true);
    }

    void Update() {
        if (State == GameState.Title) {
            if (Input.GetKeyDown(KeyCode.Return)) 
                StartGame();
            return;
        }
        if (IsPlaying) {
            timeRemaining -= Time.deltaTime * timerSpeed;
            if (timeRemaining < 0f)
                timeRemaining = 0f;
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
        if (timerText != null) {
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString("000");
        }
    }

    private void HideGameplayUI() {
        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

        if (shieldText != null)
            shieldText.gameObject.SetActive(false);

        if (oneShotText != null)
            oneShotText.gameObject.SetActive(false);

        if (timerText != null)
            timerText.gameObject.SetActive(false);
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
            gameOverScoreText.text = "Score: " + Score + "\n\'R' to Restart";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("GAME OVER. Score: " + Score);
    }
    
    public void Restart() {
        restartDirectly = true;
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }

    public void Win() {
        if (!IsPlaying) 
            return;
        timeBonus = Mathf.CeilToInt(timeRemaining);

        State = GameState.Won;
        HideGameplayUI();
        UpdateUI();

        if (winTitleText != null)
                winTitleText.text = "YOU WIN!";
        if (winScoreText != null) 
            winScoreText.text = "Score: " + Score + "\nTime Bonus: +" + timeBonus + "\n\'R' to Restart";
        if (winPanel != null) 
            winPanel.SetActive(true);

         Debug.Log("YOU WIN! Score: " + Score);
    }
}
