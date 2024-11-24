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

    public override bool TryAddPile(ItemPile pile)
    {
        if (CurrentPileUI == null && pile != null)
        {
            Debug.Log("Spawning pile in hand");
            bool success = base.TryAddPile(pile);
            if (success) { handsManager?.InstantiateItemPileInHand(pile, hand); }
            return success;
        }

        return base.TryAddPile(pile);
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
