using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Invector.vCharacterController;
using UnityEditor.Animations;
using UnityEngine.Events;
using GoTF.GameLoading;
using GoTF.Config;

namespace GoTF.Content
{
    public class PlayerManager : MonoBehaviour, IDamageable, ISaveable
    {

        #region Variables
        public DigestiveSystem DigestiveSystem { get; private set; }
        private TextMeshProUGUI interactionText;
        public PlayerStatus PlayerStatus { get; private set; }
        private Transform playerHead;
        [SerializeField] private float maxHealth;
        [SerializeField] private float maxFatigue;
        [SerializeField] private float maxCalories;
        public HandsManager HandsManager { get; private set; }
        [SerializeField] private string[] startingItems;
        [SerializeField] private GameObject rightHand;
        [SerializeField] private GameObject leftHand;
        public HandsManager.Hand prefHand = HandsManager.Hand.right;
        public HandsManager.HandMode CurrentHandMode
        {
            get
            {
                if (HandsManager == null) { return HandsManager.HandMode.none; }
                return HandsManager.CurrentHandMode;
            }
        }
        public HandsInventory HandsInventory { get; private set; }
        public PlayerInputConfig PlayerInputConfig { get; private set; }

        private CanvasManager canvasManager;

        public QuickSlot LeftHandQuickSlot
        {
            get
            {
                if (canvasManager == null) { return null; }
                return canvasManager.GetHandQuickSlot(HandsManager.Hand.left);
            }
        }
        public QuickSlot RightHandQuickSlot
        {
            get
            {
                if (canvasManager == null) { return null; }
                return canvasManager.GetHandQuickSlot(HandsManager.Hand.right);
            }
        }
        public QuickSlot BothHandQuickSlot
        {
            get
            {
                if (canvasManager == null) { return null; }
                return canvasManager.GetHandQuickSlot(HandsManager.Hand.both);
            }
        }

        private Transform itemDropper;
        private LayerMask playerLayer;
        public UpgradedThirdPersonInput InputManager { get; private set; }
        public UpgradedThirdPersonController PlayerController { get; private set; }
        public AnimatorController AnimatorController { get; private set; }

        public UnityEvent PlayerReady = new();

        [HideInInspector] public bool isInteracting;

        #endregion


        public void InitializePlayer()
        {
            itemDropper = transform.Find("Item Dropper");
            PlayerStatus = new PlayerStatus(this, maxHealth, maxFatigue, maxCalories);
            DigestiveSystem = new DigestiveSystem(PlayerStatus);
            PlayerInputConfig = new();
            HandsManager = new HandsManager(leftHand, rightHand, prefHand);
            playerLayer = LayerMask.GetMask("Player");
            playerHead = transform.Find("PlayerHead");
            InputManager = GetComponent<UpgradedThirdPersonInput>();
            AnimatorController = GetComponent<AnimatorController>();
            PlayerController = GetComponent<UpgradedThirdPersonController>();

            PlayerReady.AddListener(OnPlayerReady);
        }

        void Start()
        {
            InputManager.cameraLocked = false;
            InputManager.canAction = true;
            InputManager.canMove = true;
        }

        public void SetCanvasManager(CanvasManager canvasManager)
        {
            this.canvasManager = canvasManager;
            canvasManager.InitializeCanvasManager(this);
            HandsInventory = new HandsInventory(this);
            canvasManager.SetHandModeUI(CurrentHandMode);
        }

        public void OnPlayerReady()
        {
            //Test starting inventory :
            ItemPile pile = new ItemPile(new List<string>() { "stone", "knapped_stone", "stone" });
            HandsInventory.TryAddItemPileToNextHand(pile);
            Debug.Log("Player Ready !");
        }


        void Update()
        {
            HandleInteractions();
            HandleKeyInputs();

            //Quick and dirty for testing : to change
            if (!PlayerStatus.CanSprint)
            {
                SetWalkByDefault(true);
            }
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

                    if (Input.GetKeyDown(PlayerInputConfig.GetKeyCodeForControl(Controls.Collect)))
                    {
                        selectionTransform.GetComponent<ItemInWorld>().PickUpItem(this);
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
            if (Input.GetKeyDown(KeyCode.M))
            {
                PlayerInputConfig.SaveToJson();
            }

            InputManager.cameraLocked = hasSomeUIOpen();
            InputManager.canAction = !hasSomeUIOpen();
            InputManager.canMove = !hasSomeUIOpen();
            if (hasSomeUIOpen())
            {
                InputManager.TryStopAllActions();
            }
        }

        public void SetWalkByDefault(bool walkByDefault)
        {
            PlayerController.freeSpeed.walkByDefault = walkByDefault;
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
            return HandsInventory.TryAddItemToNextHand(itemObject.item);
        }

        public bool TryCollectPile(ItemPileInWorld itemPileInWorld)
        {
            return HandsInventory.TryAddItemPileToNextEmptyHand(itemPileInWorld.ItemPile);
        }

        public void SetHandMode(HandsManager.HandMode handMode)
        {
            Debug.Log("Setting Hand Mode to " + handMode.ToString());
            if (HandsInventory.TrySetHandModes(handMode))
            {
                canvasManager.SetHandModeUI(handMode);
                HandsManager.SetHandModes(handMode);
            }
        }

        public void SwitchHandMode()
        {
            if (HandsManager.CurrentHandMode == HandsManager.HandMode.single) { SetHandMode(HandsManager.HandMode.both); }
            else if (HandsManager.CurrentHandMode == HandsManager.HandMode.both) { SetHandMode(HandsManager.HandMode.single); }
        }

        public void SaveToJson()
        {

        }

        public void LoadFromJson()
        {

        }
    }
}
