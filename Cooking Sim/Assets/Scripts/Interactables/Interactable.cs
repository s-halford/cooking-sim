using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact()
    {
        //print("Interacting with " + transform.name);
    }

    public virtual void Interact(Inventory inventory)
    {
        //print("Interacting with " + transform.name);
    }

    public void AddInventoryPanel()
    {
        GameObject inventoryPanelPrefab = GameplayManager.instance.inventoryPanelPrefab;
        GameObject inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        InventoryUI inventoryUI = inventoryPanel.GetComponent<InventoryUI>();
        inventoryUI.inventory = GetComponent<Inventory>();

        Vector3 inventoryPoint = transform.position + new Vector3(0.5f, -0.5f);
        Vector3 targetPos = Camera.main.WorldToScreenPoint(inventoryPoint);
        inventoryPanel.transform.position = targetPos;
    }

    public GameObject AddInventoryPanel(Inventory inventory)
    {
        GameObject inventoryPanelPrefab = GameplayManager.instance.inventoryPanelPrefab;
        GameObject inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        InventoryUI inventoryUI = inventoryPanel.GetComponent<InventoryUI>();
        inventoryUI.inventory = inventory;

        Vector3 inventoryPoint = transform.position + new Vector3(0.5f, -0.5f);
        Vector3 targetPos = Camera.main.WorldToScreenPoint(inventoryPoint);
        inventoryPanel.transform.position = targetPos;
        return (inventoryPanel);
    }
}
