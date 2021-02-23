using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableSpawner : Interactable
{
    private List<VegMap> sourceVeggies;
    private Vegetable vegetable;
    private Inventory inventory;
    private IEnumerator coolDown;
    private int spawnDelayTime;

    [SerializeField] private SpriteRenderer spriteRenderer;
  
    private void Start()
    {
        spawnDelayTime = GameplayManager.instance.vegetableSpawnDelayTime;
        sourceVeggies = GameplayManager.instance.sourceVeggies;
        SpawnVegetable();
    }

    void SpawnVegetable()
    {
        vegetable = sourceVeggies[Random.Range(0, sourceVeggies.Count)].whole;
        spriteRenderer.sprite = vegetable.sprite;
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
        if(inventory != null)
        {
            if(inventory.Add(vegetable))
            {
                vegetable = null;
                spriteRenderer.sprite = null;

                if (coolDown != null)
                    StopCoroutine(coolDown);

                coolDown = CooldownRoutine();
                StartCoroutine(coolDown);
            }
        }
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(spawnDelayTime);

        if(!vegetable)
            SpawnVegetable();
    }
}
