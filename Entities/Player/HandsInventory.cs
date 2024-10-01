using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline;
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

    private Hand prefHand
    {
        get
        {
            return handsManager.PrefHand;
        }
    }

    private Hand otherHand
    {
        get
        {
            return handsManager.OtherHand;
        }
    }

    private HandMode CurrentHandMode
    {
        get
        {
            if (handsManager == null) { return HandMode.none; }
            return handsManager.CurrentHandMode;
        }
    }

    private float CurrentCarryingWeightInHand(Hand hand)
    {
        ItemPile pileInHand = ItemPileInHand(hand);
        if (pileInHand != null) { return pileInHand.Weight; }
        return 0f;
    }

    private float MaxCarryingWeight(Hand hand)
    {
        if (hand == prefHand) { return playerStatus.maxCarriyngWeightPrefHand; }
        else if (hand == otherHand) { return playerStatus.maxCarriyngWeightOtherHand; }
        else if (hand == Hand.both) { return playerStatus.maxCarriyngWeightBothHands; }
        return 0f;
    }
    private float MaxCarryingBulk(Hand hand)
    {
        if (hand == prefHand) { return playerStatus.maxCarriyngBulkPrefHand; }
        else if (hand == otherHand) { return playerStatus.maxCarriyngBulkOtherHand; }
        else if (hand == Hand.both) { return playerStatus.maxCarriyngBulkBothHands; }
        return 0f;
    }
    public float CurrentTotalCarryingWeight
    {
        get
        {
            if (handsManager.ActiveHands.Count == 0) { return 0f; }
            return handsManager.ActiveHands.Sum(hand => CurrentCarryingWeightInHand(hand));
        }
    }

    public HandsInventory(PlayerManager player)
    {
        this.player = player;
        leftHandQuickSlot = player.LeftHandQuickSlot;
        rightHandQuickSlot = player.RightHandQuickSlot;
        bothHandQuickSlot = player.BothHandQuickSlot;
        handsManager = player.HandsManager;
        playerStatus = player.PlayerStatus;
    }


    public QuickSlot HandQuickSlot(Hand hand)
    {
        return hand switch
        {
            Hand.left => leftHandQuickSlot,
            Hand.right => rightHandQuickSlot,
            Hand.both => bothHandQuickSlot,
            _ => null,
        };
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
        foreach (Hand hand in handsManager.ActiveHands)
        {
            if (IsHandEmpty(hand)) { return hand; }
        }
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
        foreach (Hand hand in handsManager.ActiveHands)
        {
            if (TryAddItemPileToHand(pile, hand)) { return true; }
        }
        return false;
    }
    public bool TryAddItemPileToHand(ItemPile pile, Hand hand)
    {
        Debug.Log("Trying to add pile to hand " + hand);
        if (!IsHandEmpty(hand)) { return ItemPileInHand(hand).TryMergePile(pile, MaxCarryingWeight(hand), MaxCarryingBulk(hand)); }
        else
        {
            AddItemPileToHand(hand, pile);
            return true;
        }
    }

    public bool TryAddItemToHand(GeneralItem item, Hand hand)
    {
        return TryAddItemPileToHand(new ItemPile(item), hand);
    }
    public bool TryAddItemToNextHand(GeneralItem item)
    {
        return TryAddItemPileToNextHand(new ItemPile(item));
    }

    public void MakeHandEmpty(Hand hand)
    {
        HandQuickSlot(hand).RemovePile();
    }

    public void MakeBothHandsEmpty()
    {
        foreach (Hand hand in handsManager.ActiveHands) { MakeHandEmpty(hand); }
    }

    private void MergeBothHands()
    {
        // Get pile in left hand and pile in right hand. Merge them and affect them to "both" hands
        ItemPilesUtilities.TryMergePiles(ItemPileInHand(prefHand), ItemPileInHand(otherHand), out ItemPile bothHandPile, maxWeight: playerStatus.maxCarriyngWeightBothHands, maxBulk: playerStatus.maxCarriyngBulkBothHands);
        MakeBothHandsEmpty();
        AddItemPileToHand(Hand.both, bothHandPile);
        Debug.Log(bothHandPile.ToString());
    }

    private bool TrySplitHands()
    {
        if (IsHandEmpty(Hand.both)) { return true; }

        List<ItemPile> splittedPile = ItemPileInHand(Hand.both).SplitItemPile(
            out ItemPile rejectedPile, 
            maxNumberOfPiles: 2, 
            maxWeight: MaxCarryingWeight(prefHand), 
            maxBulk: MaxCarryingBulk(prefHand), 
            ItemPilesUtilities.SplitMethod.Both);

        if (rejectedPile.NumberOfItemsInPile > 0) { return false; }

        else 
        {
            MakeBothHandsEmpty();
            if (splittedPile.Count > 0) { AddItemPileToHand(prefHand, splittedPile[0]); }
            if (splittedPile.Count > 1) { AddItemPileToHand(otherHand, splittedPile[1]); }
            return true; 
        }
    }

    public bool TrySetHandModes(HandMode handMode)
    {
        if (CurrentHandMode == handMode) { return true; }

        switch (handMode)
        {
            case HandMode.single:
                return TrySplitHands();

            case HandMode.both:
                MergeBothHands();
                return true;

            default: return false;
        }
    }
}
