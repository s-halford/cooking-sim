﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Used to determine whether in an inventory contains whole (unchopped) vegetables, chopped vegetables, or is empty
public enum VegetableState { Whole, Chopped, None };

public class ChoppingBoard : Interactable
{
    public delegate void OnChop(bool isChopping, Transform player);
    public OnChop onChopCallback;

    private Inventory boardInventory;
    private Inventory playerInventory;
    private IEnumerator chop;
    private int chopTime;
    private VegetableState playerVeggies;
    private VegetableState choppingBoardVeggies;

    private void Start()
    {
        // Set up the inventory panel and retrieve value we will need to determine how long chopping will take
        boardInventory = GetComponent<Inventory>();
        chopTime = GameplayManager.instance.chopTime;
        if (boardInventory != null) AddInventoryPanel(boardInventory);
    }

    public override void Interact(Inventory playerInventory)
    {
        base.Interact(playerInventory);
        this.playerInventory = playerInventory;

        playerVeggies = GetPlayerVeggieState();
        choppingBoardVeggies = GetChoppingBoardVeggieState();

        switch (playerVeggies)
        {
            case VegetableState.Whole:
                // If the player is carrying a whole vegetable and the chopping board doesn't have any whole vegetables, place the vegetable on the chopping board.
                if(choppingBoardVeggies != VegetableState.Whole)
                    DropVegetable();
                break;
            case VegetableState.Chopped:
                // If the player is carrying a salad (chopped veggies), and the chopping board is empty, player can place salad back down on chopping board.
                if(choppingBoardVeggies == VegetableState.None)
                {
                    DropSalad();
                }
                break;
            case VegetableState.None:
                // If the player isn't carrying anything and there is a whole (unchopped) vegetable on the chopping board, chop the vegetable.
                if (choppingBoardVeggies == VegetableState.Whole)
                {
                    ChopVegetable();
                } else
                {
                    // If the player isn't carrying anything, and there chopped vegetables on the chopping board, pick them up.
                    PickupSalad();
                }
                break;
        }
    }

    // Determine if player is carrying whole veggies, chopped veggies (salad), or is empty handed
    private VegetableState GetPlayerVeggieState()
    {
        if (playerInventory.veggies.Count == 0)
        {
            return VegetableState.None;
        } else
        {
            Vegetable vegetable = playerInventory.veggies[0];
            return vegetable.isChopped ? VegetableState.Chopped : VegetableState.Whole;
        }
    }

    // Determine if the chopping board has any whole veggies (meaning there's a veggie waiting to be chopped)
    // or whether the chopping board has chopped veggies or is empty
    private VegetableState GetChoppingBoardVeggieState()
    {
        if (boardInventory.veggies.Count == 0)
        {
            return VegetableState.None;
        }
        else
        {
            List<Vegetable> wholeVeggies = boardInventory.veggies.Where(t => t.isChopped == false).ToList();
            return wholeVeggies.Count > 0 ? VegetableState.Whole : VegetableState.Chopped;
        }
    }

    // Drop the vegetable the player is carrying on chopping board
    // Player will drop the first vegetable in their inventory (i.e. the first one they collected)
    private void DropVegetable()
    {
        Vegetable activeVegetable = playerInventory.veggies[0];
        boardInventory.Add(activeVegetable);
        playerInventory.Remove(activeVegetable);
    }

    // Drop the salad the player is carrying on the chopping board
    // in case customer picked up too early and must add additional ingredients
    private void DropSalad()
    {
        foreach (Vegetable vegetable in playerInventory.veggies)
        {
            boardInventory.Add(vegetable);
        }

        playerInventory.Clear();
    }

    // Pickup the salad on the chopping board
    private void PickupSalad()
    {
        foreach(Vegetable vegetable in boardInventory.veggies)
        {
            playerInventory.Add(vegetable);
        }

        boardInventory.Clear();
    }

    // Chope the vegetable on the chopping board
    private void ChopVegetable()
    {
        if (chop != null)
            StopCoroutine(chop);

        chop = ChopRoutine();
        StartCoroutine(chop);
    }

    private IEnumerator ChopRoutine()
    {
        if (onChopCallback != null)
            onChopCallback.Invoke(true, playerInventory.gameObject.transform);
        yield return new WaitForSeconds(chopTime);
        if (onChopCallback != null)
            onChopCallback.Invoke(false, playerInventory.gameObject.transform);
        
        Vegetable choppedVeggie = boardInventory.veggies[boardInventory.veggies.Count - 1];
        boardInventory.Chop(choppedVeggie);
    }
}
