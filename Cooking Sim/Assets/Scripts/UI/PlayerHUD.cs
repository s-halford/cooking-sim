using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public Transform player;

    private GameplayManager gameplayManager;

    void Start()
    {
        gameplayManager = GameplayManager.instance;

        // Add callbacks for score or timer changes
        gameplayManager.onScoreChangedCallback += UpdateScore;
        gameplayManager.onTimerChangedCallback += UpdateTimer;
        timerText.text = gameplayManager.playerTime.ToString();
        scoreText.text = gameplayManager.defaultScore.ToString();
    }

    void UpdateScore(int score, Transform player)
    {
        if (this.player != player) return;
        scoreText.text = score.ToString();
    }

    void UpdateTimer(int time, Transform player)
    {
        if (this.player != player) return;
        timerText.text = time.ToString();
    }
}
