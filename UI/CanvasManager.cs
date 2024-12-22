using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GoTF.Content
{
    public class CanvasManager : MonoBehaviour
    {
        private HealthBar healthBar;
        private CaloriesBar caloriesBar;

        private StatusBar[] allStatusBar;
        private BasicUI statusBarArea;

        private QuickSlot leftHandQuickSlot;
        private QuickSlot rightHandQuickSlot;
        private QuickSlot bothHandQuickSlot;
        private BasicUI interactionInfoUI;

        private BasicUI leftHandQuickSlotHolder;
        private BasicUI rightHandQuickSlotHolder;
        private BasicUI bothHandQuickSlotHolder;
        private QuickSlot[] allQuickSlots;

        private PileMenu pileMenuTemplate;

        public PlayerManager player { get; private set; }

        private TextMeshProUGUI interactionText;

        public UnityEvent OnCanvasReady = new();

        public void InitializeCanvasManager(PlayerManager player)
        {
            this.player = player;
            GetSubComponents();
            InitializeComponents();
            Debug.Log("Canvas Manager Successfully Initialized !");
        }

        private void GetSubComponents()
        {
            statusBarArea = transform.Find("StatusBarArea").GetComponent<BasicUI>();
            healthBar = transform.GetComponentInChildren<HealthBar>();
            caloriesBar = transform.GetComponentInChildren<CaloriesBar>();
            allStatusBar = new StatusBar[] { healthBar, caloriesBar };

            interactionInfoUI = transform.Find("Interaction_info_UI").GetComponent<BasicUI>();
            interactionText = interactionInfoUI.GetComponent<TextMeshProUGUI>();

            leftHandQuickSlotHolder = transform.Find("Handslots/LeftHandSlotHolder").GetComponent<BasicUI>();
            rightHandQuickSlotHolder = transform.Find("Handslots/RightHandSlotHolder").GetComponent<BasicUI>();
            bothHandQuickSlotHolder = transform.Find("Handslots/BothHandSlotHolder").GetComponent<BasicUI>();

            leftHandQuickSlot = transform.Find("Handslots/LeftHandSlotHolder/LeftHandSlot").GetComponent<QuickSlot>();
            rightHandQuickSlot = transform.Find("Handslots/RightHandSlotHolder/RightHandSlot").GetComponent<QuickSlot>();
            bothHandQuickSlot = transform.Find("Handslots/BothHandSlotHolder/BothHandSlot").GetComponent<QuickSlot>();
            allQuickSlots = new QuickSlot[] { leftHandQuickSlot, rightHandQuickSlot, bothHandQuickSlot };

            pileMenuTemplate = transform.Find("PileMenuTemplate").GetComponent<PileMenu>();
        }


        private void InitializeComponents()
        {
            foreach (StatusBar statusBar in allStatusBar) { statusBar.InitializeStatusBar(player); }
            foreach (QuickSlot quickSlot in allQuickSlots) { quickSlot.SetPlayer(player); }
        }

        public void OpenBasicUi(BasicUI uiPanel)
        {

        }

        public StatusBar GetStatusBar(Type statusBar)
        {
            if (statusBar == typeof(HealthBar)) { return healthBar; }
            else if (statusBar == typeof(CaloriesBar)) { return caloriesBar; }
            return null;
        }

        public QuickSlot GetHandQuickSlot(HandsManager.Hand hand)
        {
            switch (hand)
            {
                default: return null;
                case (HandsManager.Hand.left): return leftHandQuickSlot;
                case (HandsManager.Hand.right): return rightHandQuickSlot;
                case (HandsManager.Hand.both): return bothHandQuickSlot;
            }
        }

        public void SetInteractionUIActive(bool active)
        {
            interactionInfoUI.SetActive(active);
        }

        public void SetInteractionText(string text)
        {
            interactionText.text = text;
        }

        public void SetInteractionTextAndActivate(string text)
        {
            SetInteractionText(text);
            SetInteractionUIActive(true);
        }

        public void SetHandModeUI(HandsManager.HandMode handMode)
        {
            switch (handMode)
            {
                default: return;
                case HandsManager.HandMode.single:
                    {
                        bothHandQuickSlotHolder.CloseUI();
                        leftHandQuickSlotHolder.OpenUI();
                        rightHandQuickSlotHolder.OpenUI();
                        return;
                    }
                case HandsManager.HandMode.both:
                    {
                        bothHandQuickSlotHolder.OpenUI();
                        leftHandQuickSlotHolder.CloseUI();
                        rightHandQuickSlotHolder.CloseUI();
                        return;
                    }
            }
        }

        public PileMenu OpenPileMenu(ItemPileInInventory pileInInventory, Vector2 positionInUI)
        {
            PileMenu pileMenu = Instantiate(pileMenuTemplate);
            pileMenu.SetActive(true);
            pileMenu.transform.SetParent(transform, false);
            pileMenu.SetPosition(positionInUI);
            pileMenu.SetPile(pileInInventory);
            return pileMenu;
        }
    }
}
