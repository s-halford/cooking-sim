using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableSpawner : Interactable
{
    //private SpriteRenderer spriteRenderer;
    private Vegetable vegetable;
    private Inventory inventory;
    private IEnumerator coolDown;

    [SerializeField] private Vegetable[] sourceVegetables;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int timer;

    private void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        SpawnVegetable();
    }

    void SpawnVegetable()
    {
        vegetable = sourceVegetables[Random.Range(0, sourceVegetables.Length)];
        spriteRenderer.sprite = vegetable.defaultSprite;
    }

    public override void Interact()
    {
        base.Interact();

        PickUp();
    }

    public override void Interact(Inventory inventory)
    {
        base.Interact(inventory);
        this.inventory = inventory;

        if(vegetable)
            PickUp();
    }

    void PickUp()
    {
        Debug.Log("Picking up " + vegetable.itemName);

        if(inventory != null)
        {
            inventory.Add(vegetable);
            vegetable = null;
            spriteRenderer.sprite = null;

            if (coolDown != null)
                StopCoroutine(coolDown);

            coolDown = CooldownRoutine();
            StartCoroutine(coolDown);
        }
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(timer);

        if(!vegetable)
            SpawnVegetable();
    }
}
