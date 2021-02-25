using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBoost : Powerup
{
    // Value for how many additional seconds will be added to player's timer upon collecting this powerup
    [SerializeField] private float timeBonus = 20f;

    // If player picked up this item, increase the amount of time in player's timer
    public override void Pickup(Transform player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.IncreaseTime(timeBonus);
    }
}
