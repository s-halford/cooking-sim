using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    // Used to keep track of who is elgible for pickup of item
    public Transform owner;

    // Check if customer is eligible to pikcup item, if so, handle pickup and destroy object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && owner == collision.gameObject.transform)
        {
            Pickup(collision.transform);
            Destroy(gameObject);
        }

    }

    // Handle pickup of powerup
    // Meant to be overwritten as all powerups will inherit from this class
    public virtual void Pickup(Transform player)
    {
        
    }

}
