using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Plate : Interactable
{
    private Inventory plateInventory;
    private Inventory playerInventory;
    private VegetableState playerVeggies;

    void Start()
    {
        plateInventory = GetComponent<Inventory>();
        if (plateInventory != null) AddInventoryPanel();
    }

    public override void Interact(Inventory inventory)
    {
        base.Interact(inventory);

        playerInventory = inventory;
        playerVeggies = GetPlayerVeggieState();

        bool isPlateEmpty = plateInventory.veggies.Count == 0;

        if (playerVeggies == VegetableState.Whole && isPlateEmpty) DropVegetable();
        if (playerVeggies == VegetableState.None && !isPlateEmpty) PickupVegetable();

    }

    private VegetableState GetPlayerVeggieState()
    {
        if (playerInventory.veggies.Count == 0)
        {
            return VegetableState.None;
        }
        else
        {
            List<Vegetable> wholeVeggies = playerInventory.veggies.Where(t => t.isChopped == false).ToList();
            return wholeVeggies.Count > 0 ? VegetableState.Whole : VegetableState.Chopped;
        }
    }

    private void DropVegetable()
    {
        Vegetable activeVegetable = playerInventory.veggies[0];
        plateInventory.Add(activeVegetable);
        playerInventory.Remove(activeVegetable);
    }

    private void PickupVegetable()
    {
        Vegetable activeVegetable = plateInventory.veggies[0];
        playerInventory.Add(activeVegetable);
        plateInventory.Remove(activeVegetable);
    }


}
