using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableSpawner : Interactable
{
    private List<VegMap> sourceVeggies;
    private Vegetable vegetable;
    private Inventory playerInventory;
    private IEnumerator coolDown;
    private int spawnDelayTime;
    private int maxWholeVegetables;

    [SerializeField] private SpriteRenderer spriteRenderer;
  
    private void Start()
    {
        spawnDelayTime = GameplayManager.instance.vegetableSpawnDelayTime;
        sourceVeggies = GameplayManager.instance.sourceVeggies;
        maxWholeVegetables = GameplayManager.instance.maxWholeVegetables;
        SpawnVegetable();
    }

    void SpawnVegetable()
    {
        vegetable = sourceVeggies[Random.Range(0, sourceVeggies.Count)].whole;
        spriteRenderer.sprite = vegetable.sprite;
    }

    public override void Interact(Inventory playerInventory)
    {
        base.Interact(playerInventory);
        this.playerInventory = playerInventory;

        if (vegetable && playerInventory.veggies.Count < maxWholeVegetables)
            PickUp();
    }

    void PickUp()
    {
        if(playerInventory != null)
        {
            if(playerInventory.Add(vegetable))
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
