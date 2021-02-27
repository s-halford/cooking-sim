using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

// Data structure containing whole and chopped vegetable states.  Serializable so we can easily set up our vegetable list in editor.
[System.Serializable]
public struct VegMap
{
    public Vegetable whole;
    public Vegetable chopped;
}

[System.Serializable]
public class PlayerData
{
    public Transform player;
    public string name;
    public int score;
    public int time;
}

public class GameplayManager : MonoBehaviour
{
    #region Singleton

    public static GameplayManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GameplayManager found");
            return;
        }
        instance = this;
    }

    #endregion

    public delegate void OnScoreChanged(int score, Transform player);
    public OnScoreChanged onScoreChangedCallback;

    public delegate void OnTimerChanged(int time, Transform player);
    public OnTimerChanged onTimerChangedCallback;

    // The amount of time it takes to chop a vegetable at a chopping board
    public int chopTime = 5;

    // The amount of time it takes to respawn a vegetable after picking one up at a spawning station.
    public int vegetableSpawnDelayTime = 3;

    // The maximum number of whole vegetable a player can carry at a time.
    public int maxWholeVegetables = 2;

    // The amount of time a player starts with
    public int playerTime = 120;

    // The detault score a player starts with
    public int defaultScore = 0;

    public GameObject inventoryPanelPrefab;
    public GameObject nameTagPrefab;
    public Sprite[] customerSprites;
    public Salad[] salads;
    public Transform[] powerupPrefabs;
    public List<VegMap> sourceVeggies;
    public Dictionary<Vegetable, Vegetable> vegDict = new Dictionary<Vegetable, Vegetable>();
    public List<PlayerData> playerData;

    private List<Transform> inactivePlayers = new List<Transform>();

    private void Start()
    {
        // Convert our VegMap list to a dictionary so we can more easily find the chopped version of a vegetable by looking for a key
        vegDict = sourceVeggies.ToDictionary(x => x.whole, x => x.chopped);
    }

    // Called when a player has run out of time
    // Will remove player and check if all players have lost
    public void RemovePlayer(Transform player)
    {
        inactivePlayers.Add(player);
        if (inactivePlayers.Count == playerData.Count)
            GameOver();
    }

    // Handle Game Over
    private void GameOver()
    {
        SceneManager.LoadScene(1);
    }

    // Called whenever a scoring event occurs
    public void AddScore(int scoreToAdd, Transform player)
    {
        var thisPlayer = playerData.Where(x => x.player == player).SingleOrDefault();
        thisPlayer.score += scoreToAdd;
        onScoreChangedCallback?.Invoke(thisPlayer.score, player);
    }

    // Calledn whenever timer is updated
    public void UpdateTimer(float time, Transform player)
    {
        var thisPlayer = playerData.Where(x => x.player == player).SingleOrDefault();
        thisPlayer.time = (int)time;
        onTimerChangedCallback?.Invoke(thisPlayer.time, player);
    }
}
