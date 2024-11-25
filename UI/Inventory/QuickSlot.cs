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

    public float MaxCarryingWeight
    {
        get
        {
            return inventoryManager.MaxCarryingWeight(hand);
        }
    }
    public float MaxCarryingBulk
    {
        get
        {
            return inventoryManager.MaxCarryingBulk(hand);
        }
    }

    public override bool TryAddPile(ItemPile pile, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity)
    {
        if (maxWeight == Mathf.Infinity) {maxWeight = MaxCarryingWeight; }
        if (maxBulk == Mathf.Infinity) { maxBulk = MaxCarryingBulk; }
        if (CurrentPileUI == null && pile != null)
        {
            Debug.Log("Spawning pile in hand");
            bool success = base.TryAddPile(pile, maxWeight, maxBulk);
            if (success) { handsManager?.InstantiateItemPileInHand(pile, hand); }
            return success;
        }

        return base.TryAddPile(pile, maxWeight, maxBulk);
    }


    public override void RemovePile(bool shouldDestroy = true)
    {
        base.RemovePile(shouldDestroy);
        RemovePileFromHand();
    }

    public void RemovePileFromHand() => handsManager?.RemoveItemPileFromHand(hand); 

    public void ParentPileToHand(ItemPile pile) 
    {
        handsManager.ParentPileToHand(pile, hand);
    }
}
