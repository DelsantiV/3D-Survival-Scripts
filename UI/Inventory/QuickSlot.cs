using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlot : ItemSlot
{
    [HideInInspector] public static QuickSlot activeQuickSlot;
    private HandsManager handsManager;
    public HandsManager.Hand hand;


    public virtual void SetHandsManager(HandsManager handsManager,  HandsManager.Hand hand)
    {
        this.hand = hand;
        this.handsManager = handsManager;
    }

    public override void AddPile(ItemPile pile)
    {
        if (CurrentPileUI == null)
        {
            handsManager.InstantiateItemPileInHand(pile, hand);
        }
        base.AddPile(pile);
        // Needs to be improved: do not instantiate item if could not add
    }

    public void RemoveItemFromHands()
    {
        handsManager.RemoveItemPileFromHand(hand); 
    }
}
