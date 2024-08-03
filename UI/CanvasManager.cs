using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private HealthBar healthBar;
    private CaloriesBar caloriesBar;
    private PlayerInventoryUI playerInventoryUI;
    private CraftingUI craftingUI;
    private QuickSlot leftHandquickslot;
    private QuickSlot rightHandquickslot;
    private QuickSlot bothHandquickslot;
    private GameObject interactionInfoUI;
    public PlayerManager player { get; private set; }

    public void InitializeCanvasManager(PlayerManager player)
    {
        GetSubComponents();
        this.player = player;
        player.OnPlayerReady.AddListener(InitializeComponents);
    }

    private void GetSubComponents()
    {

    }

    public void InitializeComponents()
    {

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
            case (HandsManager.Hand.left): return leftHandquickslot;
            case (HandsManager.Hand.right): return rightHandquickslot;
            case (HandsManager.Hand.both): return bothHandquickslot;
        }
    }
}
