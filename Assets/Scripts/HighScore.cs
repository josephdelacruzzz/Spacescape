using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour {
    private static TMP_Text uiText;
    private static int score; //stores curr high score 

    public static int SCORE {
        get {
            return score;
        }
        private set {
            score = value; //updates curr high score
            //saves high score 
            PlayerPrefs.SetInt("HighScore", value); 
            PlayerPrefs.Save();
            UpdateUI();
        }
    }

    void Awake() {
        uiText = GetComponent<TMP_Text>();
        SCORE = PlayerPrefs.GetInt("HighScore", 0); //loads the high score
        UpdateUI();
    }

    public static void TrySetHighScore(int scoreToTry) {
        if (scoreToTry > SCORE) //update high score if new score is higher
            SCORE = scoreToTry;
    }

    private static void UpdateUI() {
        if (uiText != null)
            uiText.text = "High Score: " + SCORE;
    }
}