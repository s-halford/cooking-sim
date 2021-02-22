using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact()
    {
        print("Interacting with " + transform.name);
    }

    public virtual void Interact(Inventory inventory)
    {
        print("Interacting with " + transform.name);
    }
}
