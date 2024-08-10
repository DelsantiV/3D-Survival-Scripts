using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private CaloriesBar caloriesBar;

    private StatusBar[] allStatusBar;

    [SerializeField] private QuickSlot leftHandQuickSlot;
    [SerializeField] private QuickSlot rightHandQuickSlot;
    [SerializeField] private QuickSlot bothHandQuickSlot;
    [SerializeField] private BasicUI interactionInfoUI;

    [SerializeField] private BasicUI leftHandQuickSlotHolder;
    [SerializeField] private BasicUI rightHandQuickSlotHolder;
    [SerializeField] private BasicUI bothHandQuickSlotHolder;
    [SerializeField] private BasicUI StatusBarArea;
    public PlayerManager player { get; private set; }

    private TextMeshProUGUI interactionText;

    public void InitializeCanvasManager(PlayerManager player)
    {
        GetSubComponents();
        this.player = player;
        player.SetCanvasManager(this);
        player.OnPlayerReady.AddListener(InitializeComponents);
        bothHandQuickSlotHolder.CloseUI();
        interactionInfoUI.CloseUI();

        interactionText = interactionInfoUI.GetComponent<TextMeshProUGUI>();
    }

    public void GetSubComponents()
    {
        allStatusBar = new StatusBar[] { healthBar, caloriesBar};
    }


    public void InitializeComponents()
    {
        foreach (StatusBar statusBar in allStatusBar) { statusBar.InitializeStatusBar(player); }
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

    public void SetHandQuickSlots(HandsManager.HandMode handMode)
    {
        switch(handMode)
        {
            default : return; 
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

    public void SetItemPileToHand(ItemPile itemPile, HandsManager.Hand hand)
    {
        switch(hand)
        {
            case HandsManager.Hand.both:
                {
                    SetHandQuickSlots(HandsManager.HandMode.both);
                    return;
                }
            case HandsManager.Hand.left:
                {
                    SetHandQuickSlots(HandsManager.HandMode.single);
                    return;
                }
            case HandsManager.Hand.right:
                {
                    SetHandQuickSlots(HandsManager.HandMode.single);
                    return;
                }
        }
    }
}
