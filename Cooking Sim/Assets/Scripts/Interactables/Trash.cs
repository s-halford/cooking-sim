using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Interactable
{
    private Inventory playerInventory;

    public override void Interact(Inventory inventory)
    {
        base.Interact(inventory);

        playerInventory = inventory;
        playerInventory.Clear();
    }
}
