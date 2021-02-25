using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Customer : Interactable
{
    [SerializeField] private Inventory tableInventory;
    [SerializeField] private Inventory saladInventory;
    [SerializeField] private Transform saladPoint;
    [SerializeField] private StatusBar statusBar;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Inventory playerInventory;
    private Transform player;

    private Salad[] salads;
    private Salad activeSalad;

    private InventoryPanel saladPanel;
    private InventoryPanel tablePanel;
    private IEnumerator cooldown;

    private float timePerItem = 10;
    private bool isAngry = false;
    private List<Transform> badChefs = new List<Transform>();

    private int badSaladScore = -500;
    private int goodSaladScore = 500;
    private float angrySpeedMultiplier = 1f;
    
    void Start()
    {
        if (tableInventory != null) tablePanel = AddInventoryPanel(tableInventory);
        if (saladInventory != null) saladPanel = AddInventoryPanel(saladInventory);

        statusBar.onTimerCompleteCallback += HandleTimeUp;
        saladPanel.AddCallbacks();
        saladPanel.transform.position = Camera.main.WorldToScreenPoint(saladPoint.position);
        salads = GameplayManager.instance.salads;
        SpawnSalad();
    }

    public override void Interact(Inventory playerInventory)
    {
        base.Interact(playerInventory);
        this.playerInventory = playerInventory;
        this.player = playerInventory.gameObject.transform;
        
        bool isTableEmpty = tableInventory.veggies.Count == 0;

        List<Vegetable> choppedVeggies = playerInventory.veggies.Where(t => t.isChopped == true).ToList();

        if (isTableEmpty && choppedVeggies.Count > 0) DropSalad();

    }

    private void DropSalad()
    {
        foreach (Vegetable vegetable in playerInventory.veggies)
        {
            tableInventory.Add(vegetable);
        }

        playerInventory.Clear();
        CheckSalad();
    }

    private void SpawnSalad()
    {
        activeSalad = salads[Random.Range(0, salads.Length)];

        float totalTime = activeSalad.ingredients.Length * timePerItem + 30;

        statusBar.gameObject.SetActive(true);
        statusBar.StartCountdownTimer(totalTime);

        foreach (Vegetable vegetable in activeSalad.ingredients)
        {
            saladInventory.Add(vegetable);
        }

        saladPanel.inventory.Refresh();
        saladInventory.Refresh();
    }

    private void CheckSalad()
    {
        List<Vegetable> saladVeggies = saladInventory.veggies;
        List<Vegetable> tableVeggies = tableInventory.veggies;

        if(saladVeggies.Except(tableVeggies).Count() == 0 &&
            tableVeggies.Except(saladVeggies).Count() == 0)
        {
            GameplayManager.instance.AddScore(500, this.player);
            ResetStation();

            if (statusBar.percentFilled > 0.7)
                SpawnPowerup();

            print("GOOD SALAD!");
        } else
        {
            tableInventory.Clear();
            HandleAngryCustomer();
            print("BAD SALAD!");
        }
    }

    private void SpawnPowerup()
    {
        print("SPAWN POWERUP");
    }

    private void ResetStation()
    {
        tableInventory.Clear();
        saladInventory.Clear();
        activeSalad = null;
        statusBar.gameObject.SetActive(false);
        spriteRenderer.sprite = null;
        isAngry = false;
        badChefs.Clear();

        if (cooldown != null)
            StopCoroutine(cooldown);

        cooldown = CooldownRoutine();
        StartCoroutine(cooldown);
    }

    private void HandleAngryCustomer()
    {
        isAngry = true;
        if (!badChefs.Contains(player))
            badChefs.Add(player);
        statusBar.SetSpeedMultiplier(angrySpeedMultiplier);
    }

    private void HandleTimeUp()
    {
        if(isAngry && badChefs.Count > 0)
        {
            foreach(Transform badChef in badChefs)
                GameplayManager.instance.AddScore(badSaladScore * 2, badChef);
        } else
        {
            Transform[] players = GameplayManager.instance.players;
            foreach (Transform player in players)
            {
                GameplayManager.instance.AddScore(badSaladScore, player);
                
            }

        }

        ResetStation();
        print("TIME OUT!");
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(Random.Range(3f, 7f));
        Sprite[] sprites = GameplayManager.instance.customerSprites;
        Sprite customerSprite = sprites[Random.Range(0, sprites.Length)];
        spriteRenderer.sprite = customerSprite;
        statusBar.gameObject.SetActive(false);
        SpawnSalad();
    }
}
