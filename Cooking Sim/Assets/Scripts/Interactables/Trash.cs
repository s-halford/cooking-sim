using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Interactable
{
    private Inventory playerInventory;

    public override void Interact(Inventory playerInventory)
    {
        base.Interact(playerInventory);

        this.playerInventory = playerInventory;
        this.playerInventory.Clear();
    }
}
