using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All stations customer can interact with will inherit from this class
public class Interactable : MonoBehaviour
{
    
    // Meant to be overwritten by any station customer can interact with as
    // each station has their own unique interactions
    public virtual void Interact(Inventory playerInventory)
    {
    }

    // Since most stations have their own inventory used in their operations
    // each station can call this to generate an inventory panel for itself
    public InventoryPanel AddInventoryPanel(Inventory inventory)
    {
        GameObject inventoryPanelPrefab = GameplayManager.instance.inventoryPanelPrefab;
        GameObject inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        InventoryPanel panel = inventoryPanel.GetComponent<InventoryPanel>();
        panel.inventory = inventory;
        panel.AddCallbacks();

        Vector3 inventoryPoint = transform.position + new Vector3(0.5f, -0.5f);
        Vector3 targetPos = Camera.main.WorldToScreenPoint(inventoryPoint);
        inventoryPanel.transform.position = targetPos;
        return (panel);
    }
}
