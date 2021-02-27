using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Interactable
{
    private Inventory spawnerInventory;
    private Inventory playerInventory;

    private int spawnDelayTime;
    private List<VegMap> sourceVeggies;
    private int maxWholeVegetables;
    private IEnumerator coolDown;

    private GameplayManager gameplayManager;

    void Start()
    {
        gameplayManager = GameplayManager.instance;

        spawnerInventory = GetComponent<Inventory>();
        if (spawnerInventory != null) AddInventoryPanel(spawnerInventory);
        spawnDelayTime = gameplayManager.vegetableSpawnDelayTime;
        sourceVeggies = gameplayManager.sourceVeggies;
        maxWholeVegetables = gameplayManager.maxWholeVegetables;
        SpawnVegetable();
    }

    void SpawnVegetable()
    {
        Vegetable vegetable = sourceVeggies[Random.Range(0, sourceVeggies.Count)].whole;
        spawnerInventory.Add(vegetable);
    }

    public override void Interact(Inventory inventory)
    {
        base.Interact(inventory);
        
        playerInventory = inventory;

        if (playerInventory.veggies.Count < maxWholeVegetables && spawnerInventory.veggies.Count > 0)
        {
            PickupVegetable();
        }

    }

    void PickupVegetable()
    {
        Vegetable activeVegetable = spawnerInventory.veggies[0];
        playerInventory.Add(activeVegetable);
        spawnerInventory.Remove(activeVegetable);

        if (coolDown != null)
            StopCoroutine(coolDown);

        coolDown = CooldownRoutine();
        StartCoroutine(coolDown);
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(spawnDelayTime);

        if(spawnerInventory.veggies.Count == 0)
            SpawnVegetable();
    }
}
