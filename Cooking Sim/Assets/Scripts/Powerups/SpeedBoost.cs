using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Powerup
{
    // Value for how much player's speed will increase upon collecting this powerup
    [SerializeField] private float multiplier = 1.5f;

    // Value for how long the effects of this powerup will last upon collecting this powerup
    [SerializeField] private float duration = 10f;

    // If player picked up this item, increase the player speed for a duration of time
    public override void Pickup(Transform player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.SetSpeedMultiplier(multiplier, duration);
    }
}
