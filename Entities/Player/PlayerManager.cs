using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
        [SerializeField] private GameObject bothHand;
        public HandsManager.Hand prefHand = HandsManager.Hand.right;
        public HandsManager.Hand otherHand;
        public HandsManager.HandMode CurrentHandMode
        {
            get
            {
                if (HandsManager == null) { return HandsManager.HandMode.none; }
                return HandsManager.CurrentHandMode;
            }
        }

        public HandsManager.Hand ActionHand
        {
            get
            {
                return HandsManager.ActionHand;
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
        public Animator AnimatorController { get; private set; }

        public UnityEvent PlayerReady = new();

        [HideInInspector] public bool isInteracting;
        [HideInInspector] public bool isReadyForAction;

        #endregion


        public void InitializePlayer()
        {
            itemDropper = transform.Find("Item Dropper");
            PlayerStatus = new PlayerStatus(this, maxHealth, maxFatigue, maxCalories);
            DigestiveSystem = new DigestiveSystem(PlayerStatus);
            PlayerInputConfig = new();
            HandsManager = new HandsManager(leftHand, rightHand, prefHand:prefHand, bothHand:bothHand);
            otherHand = HandsManager.OtherHand;
            playerLayer = LayerMask.GetMask("Player");
            playerHead = transform.Find("PlayerHead");
            InputManager = GetComponent<UpgradedThirdPersonInput>();
            AnimatorController = GetComponent<Animator>();
            PlayerController = GetComponent<UpgradedThirdPersonController>();
            PlayerController.isHoldingBoth = (CurrentHandMode == HandsManager.HandMode.both);

            PlayerReady.AddListener(OnPlayerReady);
        }

        void Start()
        {
            InputManager.cameraLocked = false;
            InputManager.canMove = true;
            isReadyForAction = true;
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
            ItemPile pile = new ItemPile(new List<string>() { "axe_stone" });
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

        private void HandleInteractions()
        {
            if (Physics.Raycast(playerHead.position, playerHead.position - Camera.main.transform.position, out RaycastHit hit, 10, ~playerLayer))
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
                    //Needs optimization (is called every frame)
                    canvasManager.SetInteractionUIActive(false);
                }

            }
            else
            {
                //Needs optimization
                canvasManager.SetInteractionUIActive(false);
            }
        }

        private void HandleKeyInputs()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                PlayerInputConfig.SaveToJson();
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
        public void Die()
        {
            PlayerController.Die();
            Debug.Log("You died !");
        }

        public void TakeDamage(ItemProperties.DamageProperties damageProperties)
        {
            PlayerStatus.currentHealth -= damageProperties.amount;
            if (PlayerStatus.currentHealth < 0) { Die(); };

        }

        public Material GetHitParticlesMaterial()
        {
            return null;
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

        public bool TrySetHandMode(HandsManager.HandMode handMode)
        {
           if (HandsInventory.TrySetHandModes(handMode))
            {
                canvasManager.SetHandModeUI(handMode);
                HandsManager.SetHandModes(handMode);
                return true;
            }
            return false;
        }

        // Ne fonctionne pas (HandsInventory.ForceHandMode ne fonctionne pas pour le moment)
        public void ForceHandMode(HandsManager.HandMode handMode)
        {
            Debug.Log("Forcing Hand Mode to " + handMode.ToString());
            HandsInventory.ForceHandMode(handMode);
            canvasManager.SetHandModeUI(handMode);
            HandsManager.SetHandModes(handMode);
            
        }

        public bool TrySwitchHandMode()
        {
            if (HandsManager.CurrentHandMode == HandsManager.HandMode.single) { return TrySetHandMode(HandsManager.HandMode.both); }
            else if (HandsManager.CurrentHandMode == HandsManager.HandMode.both) { return TrySetHandMode(HandsManager.HandMode.single); }
            return false;
        }

        public int GetItemAnimationID(HandsManager.Hand hand)
        {
            return HandsManager.AnimationID(hand);
        }

        public void SaveToJson()
        {

        }

        public void LoadFromJson()
        {

        }

        // These two functions are called by animation events
        public void OnStartHit()
        {
            HandsManager.StartAction(ActionHand);
        }

        public void OnEndHit()
        {
            Debug.Log("Should stop detecting hits");
            HandsManager.EndAction();
        }
    }
}
