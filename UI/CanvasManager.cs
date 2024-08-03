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
}
