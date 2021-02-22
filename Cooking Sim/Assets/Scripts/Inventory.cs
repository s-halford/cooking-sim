using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int slots = 4;
    public List<Vegetable> veggies = new List<Vegetable>();

    public bool Add(Vegetable vegetable)
    {
        if(veggies.Count < slots)
        {
            veggies.Add(vegetable);

            if(onItemChangedCallback != null)
                onItemChangedCallback.Invoke();

            return true;
        } else
        {
            return false;
        }
            
    }

    public void Remove(Vegetable vegetable)
    {
        veggies.Remove(vegetable);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public void Clear()
    {
        veggies.Clear();

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
