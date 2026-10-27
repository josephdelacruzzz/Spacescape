using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public enum GameState {Title, Playing, GameOver, Won}

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Playing;
    public bool IsPlaying => State == GameState.Playing;

    [Header("Game References")]
    public PlayerController player;
    public Transform mothership;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text shieldText;
    public TMP_Text oneShotText;
    public TMP_Text timerText;

    [Header("Screens")]  
    public GameObject winPanel;
    public TMP_Text winScoreText;
    public TMP_Text winTitleText;
    public GameObject gameOverPanel;
    public TMP_Text gameOverScoreText;
    public TMP_Text gameOverTitleText;
    public GameObject titlePanel;
    public TMP_Text winHighScoreText;
    public TMP_Text gameOverHighScoreText;

    [Header("Timer")]
    public int startingTime = 999;
    public float timerSpeed = 1f;

    public int Score => heightScore + killScore + timeBonus;
    private float startY;
    private int heightScore;
    private int killScore;
    private float bestHeight;
    public float Progress { get; private set; } // progress for difficulty
    private float goalHeight;
    private static bool restartDirectly;
    private float timeRemaining;
    private int timeBonus;

    void Awake() {
        Instance = this;
        if (restartDirectly) { //start gameplay when restarting, not title screen
            State = GameState.Playing;
            restartDirectly = false;
        } else {
            State = GameState.Title;
        }
    }

    void Start() {
        timeRemaining = startingTime; //begins timer
        startY = player.transform.position.y; //starting height
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
        } else { //show correct gameplay ui
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
    }

    //configure title screen correctly
    public void StartGame() {
        if (State != GameState.Title)
            return;
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.startSound);

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

    //updates timer, score, and difficulty
    void Update() {
        //title -> start game
        if (State == GameState.Title) {
            if (Input.GetKeyDown(KeyCode.Return)) 
                StartGame();
            return;
        }
        
        //timer, score, and progress updated while playing
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

        //restart
        if (!IsPlaying && Input.GetKeyDown(KeyCode.R))
            Restart();
    }

    //score handling for UI after killing different sized asteroids
    public void AddKillScore(int points) {
        if (!IsPlaying)
            return;

        killScore += points;
        UpdateUI();
    }

    //refreshes UI with score and time in display
    private void UpdateUI() {
        if (scoreText != null)
            scoreText.text = "Score: " + Score;
        if (timerText != null) {
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString("000");
        }
    }

    //hides gameplay during different panels.
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

    //ends the game
    public void GameOver() {
        if (!IsPlaying)
            return;
        State = GameState.GameOver;
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.gameOverSound);

        //updates UI correctly
        HideGameplayUI();
        HighScore.TrySetHighScore(Score);
        UpdateUI();
        if (gameOverTitleText != null)
            gameOverTitleText.text = "GAME OVER!";
        if (gameOverScoreText != null)
            gameOverScoreText.text = "Score: " + Score +  "\nHigh Score: " + HighScore.SCORE + "\n\n\'R' to Restart";
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        Debug.Log("GAME OVER. Score: " + Score);
    }
    
    //restarts the scene.
    public void Restart() {
        restartDirectly = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //ends the game with win
    public void Win() {
        if (!IsPlaying) 
            return;
        timeBonus = Mathf.CeilToInt(timeRemaining);
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.winSound);

        //updates UI correctly by calculating bonus and displaying win results
        State = GameState.Won;
        HighScore.TrySetHighScore(Score);
        HideGameplayUI();
        UpdateUI();
        if (winTitleText != null)
            winTitleText.text = "YOU WIN!";
        if (winScoreText != null) 
            winScoreText.text = "Score: " + Score + "\nTime Bonus: +" + timeBonus + "\nHigh Score: " + HighScore.SCORE + "\n\n\'R' to Restart";
        if (winPanel != null) 
            winPanel.SetActive(true);

         Debug.Log("YOU WIN! Score: " + Score);
    }
}
