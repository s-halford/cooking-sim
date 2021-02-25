using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoost : Powerup
{
    // Value for how many additional points player will receive for collecting this powerup
    [SerializeField] private int bonusScore = 1500;

    // If player picked up this item, add score bonus to correct player
    public override void Pickup(Transform player)
    {
        base.Pickup(player);
        GameplayManager.instance.AddScore(bonusScore, player);
    }
}
