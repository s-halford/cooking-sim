using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Customer : Interactable
{
    [SerializeField] private Inventory tableInventory;
    [SerializeField] private Inventory saladInventory;
    [SerializeField] private Transform saladPoint;

    private Inventory playerInventory;
    private Transform player;

    private Salad[] salads;
    private Salad activeSalad;

    private InventoryPanel saladPanel;
    private InventoryPanel tablePanel;

    void Start()
    {
        if (tableInventory != null) tablePanel = AddInventoryPanel(tableInventory);
        if (saladInventory != null) saladPanel = AddInventoryPanel(saladInventory);
        saladPanel.AddCallbacks();
        saladPanel.transform.position = Camera.main.WorldToScreenPoint(saladPoint.position);
        salads = GameplayManager.instance.salads;
        SpawnSalad();
    }

    public override void Interact(Inventory playerInventory)
    {
        base.Interact(playerInventory);
        this.playerInventory = playerInventory;
        this.player = playerInventory.gameObject.transform;
        
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

        saladPanel.inventory.Refresh();
        saladInventory.Refresh();
    }

    private void CheckSalad()
    {
        List<Vegetable> saladVeggies = saladInventory.veggies;
        List<Vegetable> tableVeggies = tableInventory.veggies;

        if(saladVeggies.Except(tableVeggies).Count() == 0 &&
            tableVeggies.Except(saladVeggies).Count() == 0)
        {
            GameplayManager.instance.AddScore(500, this.player);
            print("SALAD WIN!");
        } else
        {
            print("SALAD FAIL!");
        }
    }
}
