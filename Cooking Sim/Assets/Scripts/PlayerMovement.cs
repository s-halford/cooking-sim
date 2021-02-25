using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Down, Left, Right };
public enum PlayerState { Walk, Interact}

public class PlayerMovement : MonoBehaviour
{
    public Buttons[] input;
    public float countdownTimer;
    public int currentScore;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rayDistance = 1f;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private GameObject inventoryPanelPrefab;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform inventoryPoint;
    [SerializeField] private StatusBar statusBar;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private Direction playerDir;
    private Inventory inventory;
    private Dictionary<Direction, Vector2> directions = new Dictionary<Direction, Vector2>();
    private GameObject inventoryPanel;
    private PlayerState currentState;
    private float timer;
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

        directions.Add(Direction.Up, Vector2.up);
        directions.Add(Direction.Down, Vector2.down);
        directions.Add(Direction.Left, Vector2.left);
        directions.Add(Direction.Right, Vector2.right);

        SetupChoppingBoards();
        AddInventoryPanel();
    }

    void UpdateTimer()
    {
        countdownTimer -= Time.deltaTime;
        GameplayManager.instance.UpdateTimer(countdownTimer, transform);
    }

    void Update()
    {
        var left = inputState.GetButtonValue(input[0]);
        var right = inputState.GetButtonValue(input[1]);
        var up = inputState.GetButtonValue(input[2]);
        var down = inputState.GetButtonValue(input[3]);
        var action = inputState.GetButtonValue(input[4]);

        UpdateTimer();

        if (currentState == PlayerState.Interact)
        {
            timer += Time.deltaTime;
            float timerPercent = timer / chopTime;
            statusBar.SetFillPercent(timerPercent);
        } else
        {
            if (right && !left)
                movement.x = 1;
            else if (left && !right)
                movement.x = -1;
            else
                movement.x = 0;

            if (up && !down)
                movement.y = 1;
            else if (down && !up)
                movement.y = -1;
            else
                movement.y = 0;

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

            if (action)
            {
                if (!isAxisInUse)
                    Interact();

                isAxisInUse = true;
            } else
            {
                isAxisInUse = false;
            }

            UpdatePanel();
            
        }
    }

    private void SetupChoppingBoards()
    {
        foreach(ChoppingBoard board in choppingBoards)
        {
            board.onChopCallback += WaitForChop;
        }
    }

    private void AddInventoryPanel()
    {
        inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        InventoryPanel panel = inventoryPanel.GetComponent<InventoryPanel>();
        panel.inventory = inventory;
    }

    private void UpdatePanel()
    {
        float targetDistance = 0f;

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
        RaycastHit2D ray = Physics2D.Raycast(centerPoint.position, directions[playerDir], rayDistance, interactableLayerMask);
        
        if(ray)
        {
            Interactable interactable = ray.collider.GetComponent<Interactable>();

            if (interactable != null)
                interactable.Interact(inventory);
        }
        
    }


    private Direction GetDirection()
    {
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
        movement.Normalize();
        rb.MovePosition(rb.position + movement * moveSpeed * speedMultiplier * Time.fixedDeltaTime);
        Debug.DrawRay(centerPoint.position, directions[playerDir] * rayDistance, Color.red);
    }

    void WaitForChop(bool isChopping, Transform playerTransform)
    {
        if (transform != playerTransform) return;

        if (isChopping)
        {
            currentState = PlayerState.Interact;
            statusBar.gameObject.SetActive(true);
            timer = 0f;
        }
        else
        {
            currentState = PlayerState.Walk;
            statusBar.gameObject.SetActive(false);
        }
    }

    public void SetSpeedMultiplier(float multiplier, float duration)
    {
        speedMultiplier = multiplier;

        if (speedBoost != null)
            StopCoroutine(speedBoost);

        speedBoost = SpeedBoostRoutine(duration);
        StartCoroutine(speedBoost);
    }

    public void IncreaseTime(float timeIncrease)
    {
        countdownTimer += timeIncrease;
    }

    private IEnumerator SpeedBoostRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
    }

}
