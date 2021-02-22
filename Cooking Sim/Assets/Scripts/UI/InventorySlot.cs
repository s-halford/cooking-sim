using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Vegetable vegetable;
    [SerializeField] private Image image;

    //private void Start()
    //{
    //    image = GetComponent<Image>();
    //}

    public void AddItem(Vegetable newVegetable)
    {
        vegetable = newVegetable;
        //image.enabled = true;
        image.sprite = vegetable.defaultSprite;
    }

    //public void ClearSlot()
    //{
    //    vegetable = null;
    //    //image.enabled = false;
    //    image.sprite = null;
    //}
}
