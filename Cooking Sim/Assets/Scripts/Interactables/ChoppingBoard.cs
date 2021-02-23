using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBoard : Interactable
{
    public delegate void OnChop(bool isChopping);
    public OnChop onChopCallback;

    private Inventory boardInventory;
    private Inventory playerInventory;
    private GameObject inventoryPanel;
    private IEnumerator chop;
    private int chopTime;

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

        if(playerInventory.veggies.Count > 0)
        {
            Vegetable activeVegetable = playerInventory.veggies[0];
            boardInventory.Add(activeVegetable);
            playerInventory.Remove(activeVegetable);

            if (chop != null)
                StopCoroutine(chop);

            chop = ChopRoutine();
            StartCoroutine(chop);
        }
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
