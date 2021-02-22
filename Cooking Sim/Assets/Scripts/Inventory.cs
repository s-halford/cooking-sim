using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Vegetable> veggies = new List<Vegetable>();

    public void Add(Vegetable vegetable)
    {
        veggies.Add(vegetable);
    }

    public void Remove(Vegetable vegetable)
    {
        veggies.Remove(vegetable);
    }
}
