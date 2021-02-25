using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] TMP_Text playerText;
    [SerializeField] TMP_Text scoreText;

    private void Start()
    {
        var playerData = GameplayManager.instance.playerData;
        int highScore = playerData.OrderByDescending(t => t.score).ToList().FirstOrDefault().score;
        var winningPlayers = playerData.Where(t => t.score == highScore).ToList();

        scoreText.text = "Score: " + highScore.ToString();

        if (winningPlayers.Count == playerData.Count)
            playerText.text = "It's a tie!";
        else
            playerText.text = winningPlayers[0].name.ToString() + " wins!";
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }
}
