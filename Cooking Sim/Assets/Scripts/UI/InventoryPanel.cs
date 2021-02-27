using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public Inventory inventory;
    [SerializeField] private GameObject slotPrefab = null;

    void Start()
    {
        AddCallbacks();
    }

    void UpdateUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < inventory.veggies.Count; i++)
        {
            GameObject prefab = Instantiate(slotPrefab);
            prefab.transform.SetParent(transform);

            InventorySlot slot = prefab.GetComponent<InventorySlot>();
            slot.AddItem(inventory.veggies[i]);
        }
    }

    public void AddCallbacks()
    {
        if (inventory != null)
            inventory.onItemChangedCallback += UpdateUI;
    }

    public void SetPosition(Vector3 position)
    {
        Vector3 inventoryPoint = transform.position + position;
        Vector3 targetPos = Camera.main.WorldToScreenPoint(inventoryPoint);
        transform.position = targetPos;
    }
}
