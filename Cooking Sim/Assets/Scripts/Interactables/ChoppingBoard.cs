using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Used to determine whether in an inventory contains whole (unchopped) vegetables, chopped vegetables, or is empty
public enum VegetableState { Whole, Chopped, None };

public class ChoppingBoard : Interactable
{
    public delegate void OnChop(bool isChopping);
    public OnChop onChopCallback;

    private Inventory boardInventory;
    private Inventory playerInventory;
    private GameObject inventoryPanel;
    private IEnumerator chop;
    private int chopTime;
    private VegetableState playerVeggies;
    private VegetableState choppingBoardVeggies;

    [SerializeField] private GameObject inventoryPanelPrefab;
    [SerializeField] private Transform inventoryPoint;

    private void Start()
    {
        boardInventory = GetComponent<Inventory>();
        chopTime = GameplayManager.instance.chopTime;
        AddInventoryPanel();
        UpdatePanel();
    }

    public override void Interact(Inventory inventory)
    {
        base.Interact(inventory);

        playerInventory = inventory;

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

    private void DropVegetable()
    {
        Vegetable activeVegetable = playerInventory.veggies[0];
        boardInventory.Add(activeVegetable);
        playerInventory.Remove(activeVegetable);
    }

    private void DropSalad()
    {
        foreach (Vegetable vegetable in playerInventory.veggies)
        {
            boardInventory.Add(vegetable);
        }

        playerInventory.Clear();
    }

    private void PickupSalad()
    {
        foreach(Vegetable vegetable in boardInventory.veggies)
        {
            playerInventory.Add(vegetable);
        }

        boardInventory.Clear();
    }

    private void ChopVegetable()
    {
        if (chop != null)
            StopCoroutine(chop);

        chop = ChopRoutine();
        StartCoroutine(chop);
    }

    private void AddInventoryPanel()
    {
        inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        InventoryUI inventoryUI = inventoryPanel.GetComponent<InventoryUI>();
        inventoryUI.inventory = boardInventory;
    }

    private void UpdatePanel()
    {
        Vector3 targetPos = Camera.main.WorldToScreenPoint(inventoryPoint.position);
        inventoryPanel.transform.position = targetPos;
    }

    private IEnumerator ChopRoutine()
    {
        if (onChopCallback != null)
            onChopCallback.Invoke(true);
        yield return new WaitForSeconds(chopTime);
        if (onChopCallback != null)
            onChopCallback.Invoke(false);
        
        Vegetable choppedVeggie = boardInventory.veggies[boardInventory.veggies.Count - 1];
        boardInventory.Chop(choppedVeggie);
    }
}
