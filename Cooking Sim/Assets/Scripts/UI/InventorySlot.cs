using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Vegetable vegetable;
    [SerializeField] private Image image;

    public void AddItem(Vegetable newVegetable)
    {
        vegetable = newVegetable;
        image.sprite = vegetable.defaultSprite;
    }
}
