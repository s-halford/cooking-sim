using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBoard : Interactable
{
    private Inventory boardInventory;
    private Inventory playerInventory;
    private GameObject inventoryPanel;
    
    [SerializeField] private GameObject inventoryPanelPrefab;
    [SerializeField] private Transform inventoryPoint;

    private void Start()
    {
        boardInventory = GetComponent<Inventory>();
        AddInventoryPanel();
        UpdatePanel();
    }

    public override void Interact(Inventory inventory)
    {
        base.Interact(inventory);

        playerInventory = inventory;
        List<Vegetable> veggies = playerInventory.veggies;
        

        foreach(Vegetable vegetable in veggies)
        {
            boardInventory.Add(vegetable);
        }

        playerInventory.Clear();

        print(playerInventory.veggies.Count);
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
}
