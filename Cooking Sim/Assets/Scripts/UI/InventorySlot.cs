using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void AddItem(Vegetable vegetable)
    {
        image.sprite = vegetable.sprite;
    }
}
