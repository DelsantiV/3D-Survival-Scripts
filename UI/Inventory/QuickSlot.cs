using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlot : ItemSlot
{
    [HideInInspector] public static QuickSlot activeQuickSlot;
    public HandsManager.Hand hand;

    private HandsInventory inventoryManager 
    { 
        get
        {
            return Player.HandsInventory;
        } 
    }

    private HandsManager handsManager
    {
        get
        {
            return Player.HandsManager;
        }
    }

    public override void AddPile(ItemPile pile)
    {
        Debug.Log("Hello");
        if (CurrentPileUI == null && pile != null)
        {
            Debug.Log("Spawning pile in hand");
            handsManager?.InstantiateItemPileInHand(pile, hand);
        }
        base.AddPile(pile);
        // Needs to be improved: do not instantiate item if could not add -> TryAddPile structure
    }


    public override void AddPile(ItemPileInInventory pileUI)
    {
        Debug.Log("Hello");
        if (CurrentPileUI == null && pileUI.ItemPile != null)
        {
            Debug.Log("Spawning pile in hand");
            handsManager?.InstantiateItemPileInHand(pileUI.ItemPile, hand);
        }
        base.AddPile(pileUI);
        // Needs to be improved: do not instantiate item if could not add -> TryAddPile structure
    }

    public override void RemovePile(bool shouldDestroy = true)
    {
        base.RemovePile(shouldDestroy);
        handsManager?.RemoveItemPileFromHand(hand);
    }

    public void ParentPileToHand(ItemPile pile) 
    {
        handsManager.ParentPileToHand(pile, hand);
    }
}
