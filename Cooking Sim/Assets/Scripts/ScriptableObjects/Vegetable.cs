using UnityEngine;

[CreateAssetMenu]
public class Vegetable : ScriptableObject
{
    public string itemName;
    public Sprite defaultSprite;
    public Sprite choppedSprite;
    public bool isChopped = false;
}
