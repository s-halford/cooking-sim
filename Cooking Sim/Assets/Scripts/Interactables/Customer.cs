using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Customer : Interactable
{
    [SerializeField] private Inventory tableInventory;
    [SerializeField] private Inventory saladInventory;
    private Inventory playerInventory;

    private Salad[] salads;
    private Salad activeSalad;

    private GameObject saladPanel;
    private GameObject tablePanel;

    void Start()
    {
        if (tableInventory != null) tablePanel = AddInventoryPanel(tableInventory);
        if (saladInventory != null) saladPanel = AddInventoryPanel(saladInventory);

        salads = GameplayManager.instance.salads;
        SpawnSalad();
    }

    public override void Interact(Inventory inventory)
    {
        base.Interact(inventory);

        playerInventory = inventory;
        
        bool isTableEmpty = tableInventory.veggies.Count == 0;

        List<Vegetable> choppedVeggies = playerInventory.veggies.Where(t => t.isChopped == true).ToList();

        if (isTableEmpty && choppedVeggies.Count > 0) DropSalad();

    }

    private void DropSalad()
    {
        foreach (Vegetable vegetable in playerInventory.veggies)
        {
            tableInventory.Add(vegetable);
        }

        playerInventory.Clear();

        CheckSalad();
    }

    private void SpawnSalad()
    {
        activeSalad = salads[Random.Range(0, salads.Length)];

        foreach (Vegetable vegetable in activeSalad.ingredients)
        {
            saladInventory.Add(vegetable);
        }

        saladInventory.Refresh();
    }

    private void CheckSalad()
    {
        List<Vegetable> saladVeggies = saladInventory.veggies;
        List<Vegetable> tableVeggies = tableInventory.veggies;

        if(saladVeggies.Except(tableVeggies).Count() == 0 &&
            tableVeggies.Except(saladVeggies).Count() == 0)
        {
            print("SALAD WIN!");
        } else
        {
            print("SALAD FAIL!");
        }
    }
}
