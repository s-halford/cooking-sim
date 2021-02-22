using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    [SerializeField] private GameObject slotPrefab;

    void Start()
    {
        inventory.onItemChangedCallback += UpdateUI;
    }

    private void Update()
    {
        //Vector3 pos = Camera
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
}
