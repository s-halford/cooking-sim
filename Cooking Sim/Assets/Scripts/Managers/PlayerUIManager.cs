using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public int currentScore = 0;
    public int currentTime;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public Transform player;

    private GameplayManager gameplayManager;

    void Start()
    {
        // Add callbacks for score or timer changes
        GameplayManager.instance.onScoreChangedCallback += AddScore;
        GameplayManager.instance.onTimerChangedCallback += UpdateTimer;
        timerText.text = GameplayManager.instance.playerTime.ToString();
        scoreText.text = currentScore.ToString();
    }

    void AddScore(int score, Transform player)
    {
        if (this.player != player) return;

        currentScore += score;
        scoreText.text = currentScore.ToString();
    }

    void UpdateTimer(float time, Transform player)
    {
        if (this.player != player) return;
        timerText.text = ((int)time).ToString();
    }
}
