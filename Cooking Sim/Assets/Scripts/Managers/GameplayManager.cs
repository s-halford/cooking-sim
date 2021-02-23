using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int chopTime = 5;
    public int vegetableSpawnDelayTime = 3;
    public ChoppingBoard[] choppingBoards;

    public List<VegMap> sourceVeggies;
    public Dictionary<Vegetable, Vegetable> vegDict = new Dictionary<Vegetable, Vegetable>();
 
    private void Start()
    {
        foreach(VegMap vegMap in sourceVeggies)
        {
            vegDict.Add(vegMap.whole, vegMap.chopped);
        }
    }
}
