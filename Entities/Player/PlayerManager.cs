using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Invector.vCharacterController;
using UnityEditor.Animations;
using UnityEngine.Events;


public class PlayerManager : MonoBehaviour, IDamageable
{

    #region Variables
    public static PlayerManager Player { get; private set; }

    public DigestiveSystem DigestiveSystem { get; private set; }
    private TextMeshProUGUI interactionText;
    public PlayerStatus PlayerStatus { get; private set; }
    private Transform playerHead;
    public HandsManager HandsManager {get; private set; }
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxFatigue;
    [SerializeField] private float maxCalories;
    [SerializeField] private string[] startingItems;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private HandsManager.Hand prefHand = HandsManager.Hand.right;
    public HandsInventory HandsInventory { get; private set; }

    private CanvasManager canvasManager;

    public QuickSlot LeftHandQuickSlot
    {
        get
        {
            return canvasManager.GetHandQuickSlot(HandsManager.Hand.left);
        }
    }
    public QuickSlot RightHandQuickSlot
    {
        get
        {
            return canvasManager.GetHandQuickSlot(HandsManager.Hand.right);
        }
    }
    public QuickSlot BothHandQuickSlot
    {
        get
        {
            return canvasManager.GetHandQuickSlot(HandsManager.Hand.both);
        }
    }

    private Transform itemDropper;
    private LayerMask playerLayer;
    public UpgradedThirdPersonInput InputManager { get; private set; }
    public UpgradedThirdPersonController PlayerController { get; private set; }
    public AnimatorController AnimatorController { get; private set; }

    public UnityEvent OnPlayerReady = new();

    //public event Action OnUIChange; // not implemented yet

    [HideInInspector] public bool isInteracting;

    #endregion


    private void Awake()
    {
        Player = this;
        //inventory = new InventoryManager(numberOfInventorySlots, inventoryUI);
        //craftingManager = new CraftingManager(craftingUI, inventory);
        itemDropper = transform.Find("Item Dropper");
        PlayerStatus = new PlayerStatus(maxHealth, maxFatigue, maxCalories);
        DigestiveSystem = new DigestiveSystem(PlayerStatus);
        playerLayer = LayerMask.GetMask("Player");
        playerHead = transform.Find("PlayerHead");
        InputManager = GetComponent<UpgradedThirdPersonInput>();
        AnimatorController = GetComponent<AnimatorController>();
    }

    void Start()
    {
        //interactionText = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
        //inventoryUI.CloseUI();
        //craftingUI.CloseUI();

        //foreach (InventoryItemInfos item in startingItems) { inventory.AddItemToInventory(item.itemSO, item.itemAmount); }
        OnPlayerReady.Invoke();
        InputManager.cameraLocked = false;
        InputManager.canAction = true;
        InputManager.canMove = true;

        //Test starting inventory :
        ItemPile pile = new ItemPile(new List<string>(){"stone", "knapped_stone", "stone", "knapped_stone", "stone", "knapped_stone", "stone", "knapped_stone", "stone", "knapped_stone", "stone", "knapped_stone", "stone", "knapped_stone", "stone", "knapped_stone", "stone" });
        HandsInventory.TryAddItemPileToHands(pile);
    }

    public void SetCanvasManager(CanvasManager canvasManager)
    {
        this.canvasManager = canvasManager;
        HandsManager = new HandsManager(leftHand, rightHand, LeftHandQuickSlot, RightHandQuickSlot, BothHandQuickSlot, prefHand);
        HandsInventory = new HandsInventory(this);
    }


    void Update()
    {
        HandleInteractions();
        HandleKeyInputs();
    }

    public bool hasSomeUIOpen()
    {
        //return (inventoryUI.IsOpen() || craftingUI.IsOpen());
        return false;
    }

    private void HandleInteractions()
    {
        RaycastHit hit;
        //Debug.DrawRay(playerHead.position, - Camera.main.transform.position + playerHead.position, Color.red, 10);
        if (Physics.Raycast(playerHead.position, playerHead.position - Camera.main.transform.position, out hit, 10, ~playerLayer))
        {
            var selectionTransform = hit.transform;

            if (selectionTransform.GetComponent<ItemInWorld>())
            {
                ItemInWorld currentInteraction = selectionTransform.GetComponent<ItemInWorld>();
                canvasManager.SetInteractionTextAndActivate(currentInteraction.ObjectName);
                isInteracting = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    selectionTransform.GetComponent<ItemInWorld>().PickUpItem(Player);
                }
            }
            else
            {
                canvasManager.SetInteractionUIActive(false);
            }

        }
        else
        {
            canvasManager.SetInteractionUIActive(false);
        }
    }

    private void HandleKeyInputs()
    {
        /*
        if (Input.GetKeyDown(KeyCode.I))
        {

            if (!inventoryUI.IsOpen())
            {
                Debug.Log("Inventory opened !");
                inventoryUI.OpenUI();
            }
            else
            {
                Debug.Log("Inventory closed !");
                inventoryUI.CloseUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (!canvasManager.craftingUI.IsOpen())
            {
                Debug.Log("Crafting panel opened !");
                craftingUI.OpenUI();
                craftingManager.UpdateInventoryList();
            }
            else
            {
                Debug.Log("Crafting panel closed !");
                craftingUI.CloseUI();
            }
        }
        */

        InputManager.cameraLocked = hasSomeUIOpen();
        InputManager.canAction = !hasSomeUIOpen();
        InputManager.canMove = !hasSomeUIOpen();
        if (hasSomeUIOpen())
        {
            InputManager.TryStopAllActions();
        }
    }

    public void SpawnItemFromPlayer(GeneralItem item, int amount = 1)
    {
        if (item.ItemPrefab != null)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject itemPrefab = Instantiate(item.ItemPrefab, itemDropper.position, itemDropper.rotation);
                itemPrefab.AddComponent<ItemInWorld>();
                itemPrefab.AddComponent<Rigidbody>();
                itemPrefab.GetComponent<ItemInWorld>().item = item;
            }
        }
    }

    public void SpawnPileFromPlayer(ItemPile pile)
    {
        pile.SpawnInWorld(itemDropper.position);
    }
    //public GeneralInventoryUI GetInventoryUI() { return inventoryUI; }
    public void Die()
    {
        Debug.Log("You died !");
    }

    public void TakeDamage(float damageAmount, DamageSource damageSource)
    {
        PlayerStatus.currentHealth -= damageAmount;
        if (PlayerStatus.currentHealth < 0) { Die(); };

    }

    public bool TryEatFood(FoodItem foodItem)
    {
        return DigestiveSystem.TryAddFoodToDigestiveSystem(foodItem);
    }

    public void SetActionAnimation(AnimationClip anim)
    {
        
    }

    public bool TryCollectItem(ItemInWorld itemObject) 
    {
        return HandsInventory.TryAddItemPileToHands(new ItemPile(itemObject.item));
    }

    public void SetHandMode(HandsManager.HandMode handMode)
    {
        canvasManager.SetHandModeUI(handMode);
    }
}
