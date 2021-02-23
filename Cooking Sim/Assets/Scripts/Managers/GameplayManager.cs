using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public ChoppingBoard[] choppingBoards;
    public Vegetable[] sourceVegetables;
}
