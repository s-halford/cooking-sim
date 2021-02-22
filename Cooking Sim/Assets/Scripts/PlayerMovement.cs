using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Down, Left, Right };

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rayDistance = 1f;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private GameObject inventoryPanelPrefab;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform inventoryPoint;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private Direction playerDir;
    private Inventory inventory;
    private Dictionary<Direction, Vector2> directions = new Dictionary<Direction, Vector2>();
    private GameObject inventoryPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();

        directions.Add(Direction.Up, Vector2.up);
        directions.Add(Direction.Down, Vector2.down);
        directions.Add(Direction.Left, Vector2.left);
        directions.Add(Direction.Right, Vector2.right);

        AddInventoryPanel();
    }

    void Update()
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

    private void AddInventoryPanel()
    {
        inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        InventoryUI inventoryUI = inventoryPanel.GetComponent<InventoryUI>();
        inventoryUI.inventory = inventory;
    }

    private void UpdatePanel()
    {
        Vector3 targetPos = Camera.main.WorldToScreenPoint(inventoryPoint.transform.position);
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
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        Debug.DrawRay(centerPoint.position, directions[playerDir] * rayDistance, Color.red);
    }
}
