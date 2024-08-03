using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime;
using System;
using Invector.vCharacterController;
using static UnityEditor.Progress;
using UnityEditor.Animations;


public class PlayerManagerV2 : MonoBehaviour, IDamageable
{

    #region Variables
    public static PlayerManagerV2 Player { get; private set; }

    [HideInInspector] public InventoryManager inventory { get; private set; }
    [HideInInspector] public CraftingManager craftingManager { get; private set; }
    private DigestiveSystem digestiveSystem;
    private TextMeshProUGUI interactionText;
    private PlayerStatus playerStatus;
    private Transform playerHead;
    private HandsManager handsManager;
    private Canvas mainCanvas;

    [SerializeField] private float maxHealth;
    [SerializeField] private float maxFatigue;
    [SerializeField] private float maxCalories;
    [SerializeField] private GeneralInventoryUI inventoryUI;
    [SerializeField] private int numberOfInventorySlots;
    [SerializeField] private CraftingUI craftingUI;
    [SerializeField] private GameObject interaction_Info_UI;
    [SerializeField] private string[] startingItems;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private QuickSlot leftHandQuickSlot;
    [SerializeField] private QuickSlot rightHandQuickSlot;
    [SerializeField] private HandsManager.Hand prefHand = HandsManager.Hand.right;

    private Transform itemDropper;
    private LayerMask playerLayer;
    public UpgradedThirdPersonInput InputManager { get; private set; }
    public UpgradedThirdPersonController PlayerController { get; private set; }
    public AnimatorController AnimatorController { get; private set; }

    //public event Action OnUIChange; // not implemented yet

    [HideInInspector] public bool isInteracting;

    #endregion


    private void Awake()
    {
        Player = this;
        //inventory = new InventoryManager(numberOfInventorySlots, inventoryUI);
        //craftingManager = new CraftingManager(craftingUI, inventory);
        itemDropper = transform.Find("Item Dropper");
        playerStatus = new PlayerStatus(maxHealth, maxFatigue, maxCalories);
        digestiveSystem = new DigestiveSystem(playerStatus);
        playerLayer = LayerMask.GetMask("Player");
        playerHead = transform.Find("PlayerHead");
        rightHand = transform.Find("B-hand.L").gameObject;
        Debug.Log(rightHand);
        InputManager = GetComponent<UpgradedThirdPersonInput>();
        //handsManager = new HandsManager(leftHand, rightHand, leftHandQuickSlot, rightHandQuickSlot, prefHand);
        AnimatorController = GetComponent<AnimatorController>();
    }

    void Start()
    {
        //interactionText = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
        inventoryUI.CloseUI();
        craftingUI.CloseUI();

        //foreach (InventoryItemInfos item in startingItems) { inventory.AddItemToInventory(item.itemSO, item.itemAmount); }
    }


    void Update()
    {
        HandleInteractions();
        HandleKeyInputs();
    }

    public bool hasSomeUIOpen()
    {
        return (inventoryUI.IsOpen() || craftingUI.IsOpen());
    }

    private void HandleInteractions()
    {
        RaycastHit hit;
        //Debug.DrawRay(playerHead.position, - Camera.main.transform.position + playerHead.position, Color.red, 10);
        if (Physics.Raycast(playerHead.position, playerHead.position - Camera.main.transform.position, out hit, 10, ~playerLayer))
        {
            var selectionTransform = hit.transform;

            if (selectionTransform.GetComponent<Item>())
            {
                Item currentInteraction = selectionTransform.GetComponent<Item>();
                interactionText.text = currentInteraction.DisplayObjectName();
                interaction_Info_UI.SetActive(true);
                isInteracting = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    selectionTransform.GetComponent<Item>().PickUpItem(Player);
                }
            }
            else
            {
                interaction_Info_UI.SetActive(false);
            }

        }
        else
        {
            interaction_Info_UI.SetActive(false);
        }
    }

    private void HandleKeyInputs()
    {
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
            if (!craftingUI.IsOpen())
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

        InputManager.cameraLocked = hasSomeUIOpen();
        InputManager.canAction = !hasSomeUIOpen();
        InputManager.canMove = !hasSomeUIOpen();
        if (hasSomeUIOpen())
        {
            InputManager.TryStopAllActions();
        }
    }

    public void SpawnItemFromPlayer(GeneralItem item, int amount)
    {
        if (item.ItemPrefab != null)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject itemPrefab = Instantiate(item.ItemPrefab, itemDropper.position, itemDropper.rotation);
                itemPrefab.AddComponent<Item>();
                itemPrefab.AddComponent<Rigidbody>();
                itemPrefab.GetComponent<Item>().item = item;
            }
        }
    }

    public PlayerStatus GetPlayerStatus() { return playerStatus; }
    public InventoryManager GetInventory() { return inventory; }
    public GeneralInventoryUI GetInventoryUI() { return inventoryUI; }
    public void Die()
    {
        Debug.Log("You died !");
    }

    public void TakeDamage(float damageAmount, DamageSource damageSource)
    {
        playerStatus.currentHealth -= damageAmount;
        if (playerStatus.currentHealth < 0) { Die(); };

    }

    public DigestiveSystem GetDigestiveSystem() { return digestiveSystem; }
    public bool TryEatFood(FoodItem foodItem)
    {
        return digestiveSystem.TryAddFoodToDigestiveSystem(foodItem);
    }

    public HandsManager GetHandsManager() {  return handsManager; }

    public void SetActionAnimation(AnimationClip anim)
    {
        
    }
}
