using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerUI : MonoBehaviour
{
    // Prefab that will used to display player's inventory
    [SerializeField] private GameObject inventoryPanelPrefab = null;

    // Prefab that will used to display player's inventory
    [SerializeField] private GameObject nameTagPrefab = null;

    // Center point of player
    // Used for line of sight raycasting, determining placement of inventory HUD element
    [SerializeField] private Transform centerPoint = null;

    // Used for placement of name tag
    [SerializeField] private Transform nameTagPoint = null;

    private GameObject inventoryPanel;
    private GameObject nameTag;
    private Transform canvas;
    private Inventory inventory;
    private PlayerMovement player;
    
    private void Start()
    {
        player = GetComponent<PlayerMovement>();
        inventory = GetComponent<Inventory>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;

        AddInventoryPanel();
        AddNameTag();
    }

    private void Update()
    {
        UpdatePanel();
        UpdateNameTag();
    }

    private void AddInventoryPanel()
    {
        // Inventory panel is used to display ingredients player is carrying
        inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(canvas, false);
        InventoryPanel panel = inventoryPanel.GetComponent<InventoryPanel>();
        panel.inventory = inventory;
    }

    private void AddNameTag()
    {
        // NameTag is used to display ingredients player is carrying
        nameTag = Instantiate(nameTagPrefab);
        nameTag.transform.SetParent(canvas, false);
        var playerData = GameplayManager.instance.playerData;
        var player = playerData.Where(t => t.player == transform).FirstOrDefault();

        NameTag tag = nameTag.GetComponent<NameTag>();
        tag.UpdateName(player.name);
    }

    private void UpdatePanel()
    {
        float targetDistance = 0f;

        // Based on which direction we're facing we adjust how far away invetory panel is from player
        // This is used to simulate player carrying vegetables they picked up
        switch (player.playerDir)
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

        Vector3 targetPoint = (Vector2)centerPoint.position + (player.directions[player.playerDir] * targetDistance);
        Vector3 targetPos = Camera.main.WorldToScreenPoint(targetPoint);
        inventoryPanel.transform.position = targetPos;
    }

    private void UpdateNameTag()
    {
        nameTag.transform.position = Camera.main.WorldToScreenPoint(nameTagPoint.position);
    }
}
