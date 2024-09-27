using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandsManager;

public class HandsInventory
{
    private readonly PlayerManager player;
    private readonly QuickSlot leftHandQuickSlot;
    private readonly QuickSlot rightHandQuickSlot;
    private readonly QuickSlot bothHandQuickSlot;
    private readonly HandsManager handsManager;
    private readonly PlayerStatus playerStatus;
    private readonly Hand prefHand;
    private ItemPile RightHandPile
    {
        get
        {
            return rightHandQuickSlot.CurrentPile;
        }
    }
    private ItemPile LeftHandPile
    {
        get
        {
            return leftHandQuickSlot.CurrentPile;
        }
    }
    private ItemPile BothHandPile
    {
        get
        {
            return bothHandQuickSlot.CurrentPile;
        }
    }
    private ItemPileInInventory RightHandPileUI
    {
        get
        {
            return rightHandQuickSlot.CurrentPileUI;
        }
    }
    private ItemPileInInventory LeftHandPileUI
    {
        get
        {
            return leftHandQuickSlot.CurrentPileUI;
        }
    }
    private ItemPileInInventory BothHandPileUI
    {
        get
        {
            return bothHandQuickSlot.CurrentPileUI;
        }
    }

    private Hand otherHand
    {
        get
        {
            if (prefHand == Hand.left) { return Hand.right; }
            else if (prefHand == Hand.right) { return Hand.left; }
            return Hand.none;
        }
    }

    private float MaxCarryingWeight(Hand hand)
    {
        if (hand == prefHand) { return playerStatus.maxCarriyngWeightPrefHand; }
        else if (hand == otherHand) { return playerStatus.maxCarriyngWeightOtherHand; }
        return 0f;
    }
    private float MaxCarryingBulk(Hand hand)
    {
        if (hand == prefHand) { return playerStatus.maxCarriyngBulkPrefHand; }
        else if (hand == otherHand) { return playerStatus.maxCarriyngBulkOtherHand; }
        return 0f;
    }

    public HandsInventory(PlayerManager player)
    {
        this.player = player;
        leftHandQuickSlot = player.LeftHandQuickSlot;
        rightHandQuickSlot = player.RightHandQuickSlot;
        bothHandQuickSlot = player.BothHandQuickSlot;
        handsManager = player.HandsManager;
        playerStatus = player.PlayerStatus;
        prefHand = player.prefHand;
    }

    public QuickSlot HandQuickSlot(Hand hand)
    {
        switch (hand)
        {
            case Hand.left: return leftHandQuickSlot;
            case Hand.right: return rightHandQuickSlot;
            case Hand.both: return HandQuickSlot(prefHand);
        }
        return null;
    }
    private ItemPile ItemPileInHand(Hand hand)
    {
        return HandQuickSlot(hand).CurrentPile;
    }
    private ItemPileInInventory ItemPileUIInHand(Hand hand)
    {
        return HandQuickSlot(hand).CurrentPileUI;
    }

    public bool IsHandEmpty(Hand hand)
    {
        return HandQuickSlot(hand).IsEmpty;
    }

    public Hand GetNextEmptyHand()
    {
        if (IsHandEmpty(prefHand)) { return prefHand; }
        else if (IsHandEmpty(otherHand)) { return otherHand; }
        return Hand.none;
    }

    private void AddItemPileToHand(Hand hand, ItemPile pile)
    {
        HandQuickSlot(hand).AddPile(pile);
    }

    public bool TryAddItemPileToNextEmptyHand(ItemPile pile)
    {
        Hand nextEmptyHand = GetNextEmptyHand();
        if (nextEmptyHand == Hand.none)
        {
            return false;
        }
        return TryAddItemPileToHand(pile, nextEmptyHand);
    }

    public bool TryAddItemPileToNextHand(ItemPile pile)
    {
        if (TryAddItemPileToHand(pile, prefHand)) { return true; }
        else { return TryAddItemPileToHand(pile, otherHand); }
    }
    public bool TryAddItemPileToHand(ItemPile pile, Hand hand)
    {
        if (!IsHandEmpty(hand)) { return ItemPileInHand(hand).TryMergePile(pile, MaxCarryingWeight(hand), MaxCarryingBulk(hand)); }
        else
        {
            AddItemPileToHand(hand, pile);
            return true;
        }
    }

    public bool TryAddItemToHand(GeneralItem item, Hand hand)
    {
        ItemPile itemPileInHand = ItemPileInHand(hand);
        Debug.Log("Trying to add "+ item.ItemName + " to " + hand.ToString() + " hand ");
        if (itemPileInHand == null) { return TryAddItemPileToHand(new ItemPile(item), hand); }
        else { return ItemPileInHand(hand).TryAddItemToPile(item, MaxCarryingWeight(hand), MaxCarryingBulk(hand)); }
    }
    public bool TryAddItemToNextHand(GeneralItem item)
    {
        if (TryAddItemToHand(item, prefHand)) { return true; }
        else { return TryAddItemToHand(item, otherHand); }
    }

    public void MakeHandEmpty(Hand hand)
    {
        HandQuickSlot(hand).RemovePile();
    }

    public void MakeBothHandsEmpty()
    {
        if (handsManager.CurrentHandMode == HandMode.single)
        {
            MakeHandEmpty(prefHand);
            MakeHandEmpty(otherHand);
        }
        else
        {
            MakeHandEmpty(Hand.both);
        }
    }

    private void MergeBothHands()
    {
        // Get pile in left hand and pile in right hand. Merge them and affect them to "both" hands
        ItemPilesUtilities.TryMergePiles(ItemPileInHand(prefHand), ItemPileInHand(otherHand), out ItemPile bothHandPile, maxWeight: playerStatus.maxCarriyngWeightBothHands, maxBulk: playerStatus.maxCarriyngBulkBothHands);
        MakeBothHandsEmpty();
        AddItemPileToHand(Hand.both, bothHandPile);
    }
}
