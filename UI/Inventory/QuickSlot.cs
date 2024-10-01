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
        if (CurrentPileUI == null && pile != null)
        {
            handsManager?.InstantiateItemPileInHand(pile, hand);
        }
        base.AddPile(pile);
        // Needs to be improved: do not instantiate item if could not add -> TryAddPile structure
    }

    public override void RemovePile()
    {
        base.RemovePile();
        handsManager?.RemoveItemPileFromHand(hand);
    }
}
