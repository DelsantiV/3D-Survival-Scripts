using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoTF.Content
{
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
                return inventoryManager.MaxCarryingWeightInHand(hand);
            }
        }
        public float MaxCarryingBulk
        {
            get
            {
                return inventoryManager.MaxCarryingBulkInHand(hand);
            }
        }

        public override bool TryAddPile(ItemPile pile, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity)
        {
            if (maxWeight == Mathf.Infinity) { maxWeight = MaxCarryingWeight; }
            if (maxBulk == Mathf.Infinity) { maxBulk = MaxCarryingBulk; }
            if (CurrentPileUI == null && pile != null)
            {
                Debug.Log("Spawning pile in hand");
                bool success = base.TryAddPile(pile, maxWeight, maxBulk);
                if (success) { handsManager?.InstantiateItemPileInHand(pile, hand); } // Could do better : Instantiate only new items
                return success;
            }

            return base.TryAddPile(pile, maxWeight, maxBulk);
        }


        public override void RemovePile(bool shouldDestroy = true, bool shouldDropItems = false)
        {
            base.RemovePile(shouldDestroy);
            RemovePileFromHand(shouldDropItems);
        }

        public void RemovePileFromHand(bool shouldDropItems)
        {
            handsManager?.RemoveItemPileFromHand(hand, shouldDropItems: shouldDropItems);
        }

        public void ParentPileToHand(ItemPile pile)
        {
            handsManager.ParentPileToHand(pile, hand);
        }

        public bool TryParentPileToHand(ItemPile pile, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity)
        {
            if (maxWeight == Mathf.Infinity) { maxWeight = MaxCarryingWeight; }
            if (maxBulk == Mathf.Infinity) { maxBulk = MaxCarryingBulk; }
            if (pile != null)
            {
                if (pile.Weight < maxWeight && pile.Bulk < maxBulk)
                {
                    ParentPileToHand(pile);
                    return true;
                }
            }
            return false;
        }
    }
}
