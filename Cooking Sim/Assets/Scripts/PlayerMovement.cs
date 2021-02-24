using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Down, Left, Right };
public enum PlayerState { Walk, Interact}

public class PlayerMovement : MonoBehaviour
{
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();

        statusBar.gameObject.SetActive(false);
        currentState = PlayerState.Walk;

        chopTime = GameplayManager.instance.chopTime;
        choppingBoards = GameplayManager.instance.choppingBoards;

        directions.Add(Direction.Up, Vector2.up);
        directions.Add(Direction.Down, Vector2.down);
        directions.Add(Direction.Left, Vector2.left);
        directions.Add(Direction.Right, Vector2.right);

        SetupChoppingBoards();
        AddInventoryPanel();
    }

    void Update()
    {
        if (currentState == PlayerState.Interact)
        {
            timer += Time.deltaTime;
            float timerPercent = timer / chopTime;
            statusBar.SetFillPercent(timerPercent);
        } else
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

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

            if (Input.GetKeyDown("space"))
            {
                Interact();
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
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        Debug.DrawRay(centerPoint.position, directions[playerDir] * rayDistance, Color.red);
    }

    void WaitForChop(bool isChopping)
    {
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

        
}
