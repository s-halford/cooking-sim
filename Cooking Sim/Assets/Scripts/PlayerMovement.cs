using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Down, Left, Right };
public enum PlayerState { Walk, Interact}

public class PlayerMovement : MonoBehaviour
{
    // An array of all the buttons used by the player
    public Buttons[] input;
    public float countdownTimer;
    public int currentScore;

    [SerializeField] private float moveSpeed = 5f;

    // How far in front of customer will regiester an interaction hit
    [SerializeField] private float rayDistance = 1f;

    // Used to to determine if player can interact with object
    [SerializeField] private LayerMask interactableLayerMask;

    // Prefab that will used to display player's inventory
    [SerializeField] private GameObject inventoryPanelPrefab;

    // Center point of player
    // Used for line of sight raycasting, determining placement of inventory HUD element
    [SerializeField] private Transform centerPoint;

    // Displayed over customer's head to display how much longer they have to wait while chopping
    [SerializeField] private StatusBar statusBar;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private Direction playerDir;
    private Inventory inventory;
    private Dictionary<Direction, Vector2> directions = new Dictionary<Direction, Vector2>();
    private GameObject inventoryPanel;
    private PlayerState currentState;
    //private float timer;
    private int chopTime;
    private ChoppingBoard[] choppingBoards;
    private InputState inputState;
    private bool isAxisInUse = false;
    private float speedMultiplier = 1f;
    private IEnumerator speedBoost;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
        inputState = GetComponent<InputState>();

        statusBar.gameObject.SetActive(false);
        currentState = PlayerState.Walk;

        chopTime = GameplayManager.instance.chopTime;
        choppingBoards = GameplayManager.instance.choppingBoards;
        countdownTimer = GameplayManager.instance.playerTime;

        // Directions is a dictionary used to determine the associated vector to use for
        // for line of sight and inventory HUD element placement, as placement depends on direction player is facing
        directions.Add(Direction.Up, Vector2.up);
        directions.Add(Direction.Down, Vector2.down);
        directions.Add(Direction.Left, Vector2.left);
        directions.Add(Direction.Right, Vector2.right);

        SetupChoppingBoards();
        AddInventoryPanel();
    }

    void Update()
    {
        UpdateTimer();

        if (currentState == PlayerState.Walk) {
            movePlayer();
            checkForAction();
            UpdatePanel();
        }
    }

    private void movePlayer()
    {
        // Determine whether each of our movement buttons are pressed
        // Buttons are abstracted so each player can have unique inputs for multiplayer
        var left = inputState.GetButtonValue(input[0]);
        var right = inputState.GetButtonValue(input[1]);
        var up = inputState.GetButtonValue(input[2]);
        var down = inputState.GetButtonValue(input[3]);

        // Check if the left or right buttons have been pressed, and update our movement vector accordingly
        if (right && !left)
            movement.x = 1;
        else if (left && !right)
            movement.x = -1;
        else
            movement.x = 0;

        // Check if the up or down buttons have been pressed, and update our movement vector accordingly
        if (up && !down)
            movement.y = 1;
        else if (down && !up)
            movement.y = -1;
        else
            movement.y = 0;

        // If we have movement update the animation states and determine the direction we're facing
        if (movement != Vector2.zero)
        {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);

            playerDir = GetDirection();

            // If we're moving left set xScale to -1 to horizontally flip sideways-facing sprites
            float xScale = movement.x < 0 ? -1f : 1f;
            transform.localScale = new Vector3(xScale, 1f, 1f);
        }

        anim.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void checkForAction()
    {
        // Determine whether each of our action button has been pressed
        // Buttons are abstracted so each player can have unique inputs for multiplayer
        var action = inputState.GetButtonValue(input[4]);

        // For the action button we essentiall want it to act similary to 'getKeyDown'
        // So we use the AxisInUse bool to determine whether player just pressed button or has been holding it down
        if (action)
        {
            if (!isAxisInUse)
                Interact();

            isAxisInUse = true;
        }
        else
        {
            isAxisInUse = false;
        }
    }

    // Countdown player's timer
    void UpdateTimer()
    {
        countdownTimer -= Time.deltaTime;
        GameplayManager.instance.UpdateTimer(countdownTimer, transform);
    }

    private void SetupChoppingBoards()
    {
        // Add a callback for each chopping board
        foreach(ChoppingBoard board in choppingBoards)
        {
            board.onChopCallback += WaitForChop;
        }
    }

    private void AddInventoryPanel()
    {
        // Inventory panel is used to display ingredients player is carrying
        inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        InventoryPanel panel = inventoryPanel.GetComponent<InventoryPanel>();
        panel.inventory = inventory;
    }

    private void UpdatePanel()
    {
        float targetDistance = 0f;

        // Based on which direction we're facing we adjust how far away invetory panel is from player
        // This is used to simulate player carrying vegetables they picked up
        switch (playerDir)
        {
            case Direction.Up:
                targetDistance = 1f;
                break;
            case Direction.Down:
                targetDistance = 1f;
                break;
            default:
                targetDistance = 0.75f;
                break;
        }

        Vector3 targetPoint = (Vector2)centerPoint.position + (directions[playerDir] * targetDistance);
        Vector3 targetPos = Camera.main.WorldToScreenPoint(targetPoint);
        inventoryPanel.transform.position = targetPos;
    }

    private void Interact()
    {
        // See if we hit any interactable objects within our line of sire
        RaycastHit2D hit = Physics2D.Raycast(centerPoint.position, directions[playerDir], rayDistance, interactableLayerMask);

        // If we hit any interactable objects, interact with them
        if(hit)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
                interactable.Interact(inventory);
        }
        
    }


    private Direction GetDirection()
    {
        // Determine direction player is facing
        // This is used for line of sight functionality as well as inventory panel placement
        if (movement.y != 0)
        {
            return movement.y > 0 ? Direction.Up : Direction.Down;
        }
        else
        {
            return movement.x > 0 ? Direction.Right : Direction.Left;
        }
    }

    private void FixedUpdate()
    {
        // Move our player
        movement.Normalize();
        rb.MovePosition(rb.position + movement * moveSpeed * speedMultiplier * Time.fixedDeltaTime);
        Debug.DrawRay(centerPoint.position, directions[playerDir] * rayDistance, Color.red);
    }

    void WaitForChop(bool isChopping, Transform playerTransform)
    {
        if (transform != playerTransform) return;

        // If player started chopping, start waiting for chop to complete
        if (isChopping)
        {
            currentState = PlayerState.Interact;
            statusBar.gameObject.SetActive(true);
            statusBar.StartStopWatchTimer(chopTime);
        }
        else
        {
            // If chopping is complete, resume regulary gameplay
            currentState = PlayerState.Walk;
            statusBar.gameObject.SetActive(false);
        }
    }

    // Used by powerups to increase our speed
    public void SetSpeedMultiplier(float multiplier, float duration)
    {
        speedMultiplier = multiplier;

        if (speedBoost != null)
            StopCoroutine(speedBoost);

        speedBoost = SpeedBoostRoutine(duration);
        StartCoroutine(speedBoost);
    }

    // Used by powerups to increase our time
    public void IncreaseTime(float timeIncrease)
    {
        countdownTimer += timeIncrease;
    }

    // Used to determine how long our speed effect will last
    private IEnumerator SpeedBoostRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
    }
}
