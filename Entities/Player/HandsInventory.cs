using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GoTF.Content.HandsManager;

namespace GoTF.Content
{
    public class HandsInventory
    {
        private readonly PlayerManager player;
        private readonly QuickSlot leftHandQuickSlot;
        private readonly QuickSlot rightHandQuickSlot;
        private readonly QuickSlot bothHandQuickSlot;
        private readonly HandsManager handsManager;
        private readonly PlayerStatus playerStatus;
        private ItemPile RightHandPile;
        private ItemPile LeftHandPile;
        private ItemPile BothHandPile;
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

        private float CurrentCarryingBulkInHand(Hand hand)
        {
            ItemPile pileInHand = ItemPileInHand(hand);
            if (pileInHand != null) { return pileInHand.Bulk; }
            return 0f;
        }

        public float MaxCarryingWeightInHand(Hand hand)
        {
            if (hand == prefHand) { return playerStatus.maxCarriyngWeightPrefHand; }
            else if (hand == otherHand) { return playerStatus.maxCarriyngWeightOtherHand; }
            else if (hand == Hand.both) { return playerStatus.maxCarriyngWeightBothHands; }
            return 0f;
        }
        public float MaxCarryingBulkInHand(Hand hand)
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
            LeftHandPile = new();
            RightHandPile = new();
            BothHandPile = new();
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

        public bool CouldHoldPileInHand(ItemPile pile, Hand hand)
        {
            return CurrentCarryingWeightInHand(hand) + pile.Weight < MaxCarryingWeightInHand(hand) && CurrentCarryingBulkInHand(hand) + pile.Bulk < MaxCarryingBulkInHand(hand);
        }
        public bool CouldHoldPileInHand(GeneralItem item, Hand hand)
        {
            return CurrentCarryingWeightInHand(hand) + item.Weight < MaxCarryingWeightInHand(hand) && CurrentCarryingBulkInHand(hand) + item.Bulk < MaxCarryingBulkInHand(hand);
        }
        public ItemPile ItemPileInHand(Hand hand)
        {
            switch (hand)
            {
                case Hand.right: return RightHandPile;
                case Hand.left: return LeftHandPile;
                case Hand.both: return BothHandPile;
            }
            return null;
        }

        private void SetItemPileToHand(ItemPile pile, Hand hand)
        {
            switch (hand)
            {
                case Hand.right: RightHandPile = pile; return;
                case Hand.left: LeftHandPile = pile; return;
                case Hand.both: BothHandPile = pile; return;
            }
            Debug.Log("Cannot set pile " + pile + "to hand none");
        }

        private void AddPileToEmptyHand(ItemPile pile, Hand hand)
        {
            SetItemPileToHand(pile, hand); 
            pile.SetItemPileToSlot(HandQuickSlot(hand));
            handsManager.AddPileToHand(pile, hand);
        }

        public ItemPile PrefHandPile { get { return ItemPileInHand(prefHand); } }
        public ItemPile OtherHandPile { get { return ItemPileInHand(otherHand); } }

        private ItemPileInInventory ItemPileUIInHand(Hand hand)
        {
            return HandQuickSlot(hand).CurrentPileUI;
        }

        public bool IsHandEmpty(Hand hand)
        {
            return ItemPileInHand(hand).IsEmpty;
        }

        public void ClearHand(Hand hand)
        {
            SetItemPileToHand(new ItemPile(), hand);
        }

        public void RemovePileFromHand(Hand hand, bool shouldDrop)
        {
            ClearHand(hand);
            handsManager.RemoveItemPileFromHand(hand, shouldDrop);
        }
        public Hand GetNextEmptyHand()
        {
            foreach (Hand hand in handsManager.ActiveHands)
            {
                if (IsHandEmpty(hand)) { return hand; }
            }
            return Hand.none;
        }

        public bool DoesHandContains(Hand hand, GeneralItem item)
        {
            return ItemPileInHand(hand).Contains(item);
        }

        public bool DoesHandContains(Hand hand, ItemPile itemPile)
        {
            return ItemPileInHand(hand).Contains(itemPile);
        }

        public bool Contains(GeneralItem item)
        {
            foreach (Hand hand in handsManager.ActiveHands)
            {
                if (ItemPileInHand(hand).Contains(item)) return true;
            }
            return false;
        }

        public bool Contains(ItemPile itemPile)
        {
            foreach (Hand hand in handsManager.ActiveHands)
            {
                if (ItemPileInHand(hand).Contains(itemPile)) return true;
            }
            return false;
        }

        public Hand GetFirstHandThatContains(GeneralItem item)
        {
            foreach (Hand hand in handsManager.ActiveHands)
            {
                if (ItemPileInHand(hand).Contains(item)) return hand;
            }
            return Hand.none;
        }

        public Hand GetFirstHandThatContains(ItemPile itemPile)
        {
            foreach (Hand hand in handsManager.ActiveHands)
            {
                if (ItemPileInHand(hand).Contains(itemPile)) return hand;
            }
            return Hand.none;
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
                if (TryAddItemPileToHand(pile, hand)) 
                {
                    Debug.Log("Pile " + pile + " added to hand " + hand);
                    return true; 
                }
            }
            return false;
        }
        public bool TryAddItemPileToHand(ItemPile pile, Hand hand)
        {
            Debug.Log("Trying to add pile " + pile + " to hand " + hand);
            if (!IsHandEmpty(hand)) 
            {
                return ItemPileInHand(hand).TryMergePile(pile, MaxCarryingWeightInHand(hand), MaxCarryingBulkInHand(hand)); 
            }
            else
            {
                return TryAddItemPileToEmptyHand(pile, hand);
            }
        }

        public bool TryAddItemPileToEmptyHand(ItemPile pile, Hand hand)
        {
            if (pile == null)
            {
                Debug.Log("Trying to add null pile");
                return false;
            }
            if (!IsHandEmpty(hand))
            {
                Debug.Log("Hand is not empty !");
                return false;
            }
            if (pile.IsEmpty)
            {
                Debug.Log("Adding empty pile");
                return true;
            }
            if (pile.Weight < MaxCarryingWeightInHand(hand) && pile.Bulk < MaxCarryingBulkInHand(hand))
            {
                AddPileToEmptyHand(pile, hand);
                return true;
            }
            return false;
        }

        public bool TryAddItemToHand(GeneralItem item, Hand hand)
        {
            return TryAddItemPileToHand(new ItemPile(item), hand);
        }
        public bool TryAddItemToNextHand(GeneralItem item)
        {
            return TryAddItemPileToNextHand(new ItemPile(item));
        }

        public bool TryRemoveItem(GeneralItem item, bool shouldDestroy)
        {
            foreach (Hand hand in handsManager.ActiveHands)
            {
                if (ItemPileInHand(hand).TryRemoveItemFromPile(item, shouldDestroy)) { return true; }
            }
            return false;
        }
        public bool TryRemoveItemPile(ItemPile itemPile, bool shouldDestroy = false)
        {
            foreach(Hand hand in handsManager.ActiveHands)
            {
                if (ItemPileInHand(hand).TryRemovePile(itemPile, shouldDestroy)) return true;
            }
            return false;
        }

        public void RemoveItemFromPileInHand(Hand hand, GeneralItem item, bool shouldDestroy)
        {
            ItemPileInHand(hand).TryRemoveItemFromPile(item, shouldDestroy);
        }
        public void RemovePileFromPileInHand(Hand hand, ItemPile itemPile, bool shouldDestroy)
        {
            ItemPileInHand(hand).TryRemovePile(itemPile, shouldDestroy);
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
            if (bothHandPile != null)
            {
                TryAddItemPileToHand(bothHandPile, Hand.both);
                Debug.Log(bothHandPile.ToString());
            }
        }

        private bool TrySplitHands()
        {
            if (IsHandEmpty(Hand.both)) { return true; }

            List<ItemPile> splittedPile = ItemPileInHand(Hand.both).SplitItemPile(
                out ItemPile rejectedPile,
                maxNumberOfPiles: 2,
                maxWeight: MaxCarryingWeightInHand(prefHand),
                maxBulk: MaxCarryingBulkInHand(prefHand),
                ItemPilesUtilities.SplitMethod.Both);

            if (rejectedPile.NumberOfItemsInPile > 0) 
            {
                Debug.Log("Cannot split hands : both hands piles is too heavy or too bulky");
                return false; 
            }

            else
            {
                MakeBothHandsEmpty();
                if (splittedPile.Count > 1) { TryAddItemPileToHand(splittedPile[1], otherHand); }
                if (splittedPile.Count > 0) { TryAddItemPileToHand(splittedPile[0], prefHand); }
                return true;
            }
        }

        public void ForceSplitHands()
        {
            if (IsHandEmpty(Hand.both)) { return; }

            ItemPile prefHandPile = ItemPileInHand(Hand.both).TakePartOfPile(maxWeight: MaxCarryingWeightInHand(prefHand), maxBulk: MaxCarryingBulkInHand(prefHand), removeFromOriginalPile:true);
            if (!prefHandPile.IsEmpty) { TryAddItemPileToHand(prefHandPile, prefHand); }
            ItemPile otherHandPile = ItemPileInHand(Hand.both).TakePartOfPile(maxWeight: MaxCarryingWeightInHand(prefHand), maxBulk: MaxCarryingBulkInHand(prefHand), removeFromOriginalPile: true);
            Debug.Log(otherHandPile.ToString());
            if (!otherHandPile.IsEmpty) { TryAddItemPileToHand(otherHandPile, otherHand); }

            Debug.Log("After splitting, " + ItemPileInHand(Hand.both) + " needs to be dropped");
            if (!ItemPileInHand(Hand.both).IsEmpty) 
            { 
                handsManager.RemoveItemPileFromHand(Hand.both, shouldDropItems:true);
                //MakeBothHandsEmpty();
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

        public void ForceHandMode(HandMode handMode)
        {
            if (CurrentHandMode == handMode) { return; }

            switch (handMode)
            {
                case HandMode.single:
                    ForceSplitHands();
                    return;

                case HandMode.both:
                    MergeBothHands();
                    return;

                default: return ;
            }
        }
    }
}
