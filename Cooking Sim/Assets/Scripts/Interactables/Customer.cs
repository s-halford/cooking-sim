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

    // How much additional wait time to add for each item in customer's salad
    private float timePerItem = 10f;

    // A base amount of time customer will wait
    private float timeBuffer = 30f;

    // Used to keep track if a customer is angry (i.e. was delivered an incorrect salad)
    private bool isAngry = false;

    // Used to keep track of players who delivered an incorrect salad to customer
    private List<Transform> badChefs = new List<Transform>();

    // Points penalized for delivering incorrect salad
    private int badSaladScore = -500;

    // Points given for delivering correct salad;
    private int goodSaladScore = 500;

    // Determine how much faster angry customer's timer will count down
    private float angrySpeedMultiplier = 3f;
    
    void Start()
    {
        // Add inventory panels for the table and for customer salad
        if (tableInventory != null) tablePanel = AddInventoryPanel(tableInventory);
        if (saladInventory != null) saladPanel = AddInventoryPanel(saladInventory);

        // Setup callbacks, place inventory HUD elements in correct position, set up the station
        statusBar.onTimerCompleteCallback += HandleTimeUp;
        saladPanel.AddCallbacks();
        saladPanel.transform.position = Camera.main.WorldToScreenPoint(saladPoint.position);
        salads = GameplayManager.instance.salads;
        ResetStation(0f, 5f);
    }

    public override void Interact(Inventory playerInventory)
    {
        base.Interact(playerInventory);
        this.playerInventory = playerInventory;
        this.player = playerInventory.gameObject.transform;

        // Determine if table is empty, so we know if player can drop salad at station
        bool isTableEmpty = tableInventory.veggies.Count == 0;

        List<Vegetable> choppedVeggies = playerInventory.veggies.Where(t => t.isChopped == true).ToList();

        // If table is empty and player is carrying a salad, drop it on table
        if (isTableEmpty && choppedVeggies.Count > 0) DropSalad();

    }

    private void DropSalad()
    {
        // Drop salad in front of customer and check if the ingredients are correct
        foreach (Vegetable vegetable in playerInventory.veggies)
        {
            tableInventory.Add(vegetable);
        }

        playerInventory.Clear();
        CheckSalad();
    }

    private void SpawnSalad()
    {
        // Get a random salad from our salad list
        activeSalad = salads[Random.Range(0, salads.Length)];

        // Determine how long customer will wait for item (total time varies depending on # of ingredients
        float totalTime = activeSalad.ingredients.Length * timePerItem + timeBuffer;

        // Start the customer's countdown timer
        statusBar.gameObject.SetActive(true);
        statusBar.StartCountdownTimer(totalTime);

        // Display the salad above customer's head
        foreach (Vegetable vegetable in activeSalad.ingredients)
        {
            saladInventory.Add(vegetable);
        }
    }

    private void CheckSalad()
    {
        // Get the table's veggies and the customer's salad veggies for comparison
        List<Vegetable> saladVeggies = saladInventory.veggies;
        List<Vegetable> tableVeggies = tableInventory.veggies;

        // Compare the ingredients customer dropped on table against ingredients in customer's salad
        if(saladVeggies.Except(tableVeggies).Count() == 0 &&
            tableVeggies.Except(saladVeggies).Count() == 0)
        {
            // If the customer's salad has the correct ingredients, give them points and reset station
            GameplayManager.instance.AddScore(goodSaladScore, this.player);
            ResetStation();

            // If customer has more than 70% time left, spawn a powerup
            if (statusBar.percentFilled > 0.7)
                SpawnPowerup();
        } else
        {
            // If player submitted incorrect ingredients, player gets angry
            tableInventory.Clear();
            HandleAngryCustomer();
            Debug.Log("Incorrect salad");
        }
    }

    private void SpawnPowerup()
    {
        // Grab a random powerup from our list
        Transform[] powerupPrefabs = GameplayManager.instance.powerupPrefabs;
        Transform powerupPrefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
        var powerupItem = Instantiate(powerupPrefab);

        // Set the position somewhere randomly in our field of play
        // Whole numbers used to align with tile grid
        int xPos = Random.Range(-7, 7);
        int yPos = Random.Range(-3, 0);

        powerupItem.position = new Vector2(xPos, yPos);

        // Set the owner of the powerup (Only owner can retrieve powerup
        Powerup powerup = powerupItem.GetComponent<Powerup>();
        powerup.owner = player;
    }

    // Reset station and specift minimum and maximum wait time between customers
    private void ResetStation(float minWaitTime = 3f, float maxWaitTime = 7f)
    {
        // Reset everything at customer station
        tableInventory.Clear();
        saladInventory.Clear();
        activeSalad = null;
        statusBar.gameObject.SetActive(false);
        spriteRenderer.sprite = null;
        isAngry = false;
        badChefs.Clear();

        // Start cooldown to wait for next customer
        if (cooldown != null)
            StopCoroutine(cooldown);

        cooldown = CooldownRoutine(minWaitTime, maxWaitTime);
        StartCoroutine(cooldown);
    }

    private void HandleAngryCustomer()
    {
        // If customer is angry, and player to list of 'bad chefs', and increase speed
        // 'Bad chefs' are used to determine penalty for having delivered incorrect salad
        isAngry = true;
        if (!badChefs.Contains(player))
            badChefs.Add(player);
        statusBar.SetSpeedMultiplier(angrySpeedMultiplier);
    }

    // If the customer has run out of time...
    // This is a callback from the status bar delegate used to determine time up
    private void HandleTimeUp()
    {
        // If customer is angery when time expired, penalize 2x any player who delivered incorrect salad
        if(isAngry && badChefs.Count > 0)
        {
            foreach(Transform badChef in badChefs)
                GameplayManager.instance.AddScore(badSaladScore * 2, badChef);
        } else
        {
            // If time ran out but no player delivered incorrect salad, penalize both players
            Transform[] players = GameplayManager.instance.players;
            foreach (Transform player in players)
            {
                GameplayManager.instance.AddScore(badSaladScore, player);
                
            }

        }

        // Reset station for next customer
        ResetStation();
    }

    private IEnumerator CooldownRoutine(float minWaitTime, float maxWaitTime)
    {
        // After station has been cleared, wait for next customer
        // Random wait time between 3 and 7 seconds used to create variation between customers
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        // After waiting, generate a new random customer and salad
        Sprite[] sprites = GameplayManager.instance.customerSprites;
        Sprite customerSprite = sprites[Random.Range(0, sprites.Length)];
        spriteRenderer.sprite = customerSprite;
        statusBar.gameObject.SetActive(false);
        SpawnSalad();
    }
}
