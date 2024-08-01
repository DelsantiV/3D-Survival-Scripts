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
    private GameObject interactionInfoUI;

    private void InitializeCanvasManager(PlayerManagerV2 player)
    {
        GetSubComponents();
        InitializeComponents(player);
    }

    private void GetSubComponents()
    {

    }

    private void InitializeComponents(PlayerManagerV2 player)
    {

    }

    public void OpenBasicUi(BasicUI uiPanel)
    {

    }
}
