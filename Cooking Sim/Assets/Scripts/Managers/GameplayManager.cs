using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Data structure containing whole and chopped vegetable states.  Serializable so we can easily set up our vegetable list in editor.
[System.Serializable]
public struct VegMap
{
    public Vegetable whole;
    public Vegetable chopped;
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

    // The amount of time it takes to chop a vegetable at a chopping board
    public int chopTime = 5;
    // The amount of time it takes to respawn a vegetable after picking one up at a spawning station.
    public int vegetableSpawnDelayTime = 3;
    // The maximum number of whole vegetable a player can carry at a time.
    public int maxWholeVegetables = 2;

    public GameObject inventoryPanelPrefab;
    public ChoppingBoard[] choppingBoards;

    public List<VegMap> sourceVeggies;
    public Dictionary<Vegetable, Vegetable> vegDict = new Dictionary<Vegetable, Vegetable>();

    public Salad[] salads;
 
    private void Start()
    {
        // Convert our VegMap list to a dictionary so we can more easily find the chopped version of a vegetable by looking for a key
        vegDict = sourceVeggies.ToDictionary(x => x.whole, x => x.chopped);
    }
}
