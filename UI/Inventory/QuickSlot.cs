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

        public override ItemPile CurrentPile
        {
            get
            {
                return InventoryManager.ItemPileInHand(hand);
            }
        }

        private HandsInventory InventoryManager
        {
            get
            {
                return Player.HandsInventory;
            }
        }

        private HandsManager HandsManager
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
                return InventoryManager.MaxCarryingWeightInHand(hand);
            }
        }
        public float MaxCarryingBulk
        {
            get
            {
                return InventoryManager.MaxCarryingBulkInHand(hand);
            }
        }

        public override void RemovePile(bool shouldDestroy = true, bool shouldDropItems = false)
        {
            base.RemovePile(shouldDestroy);
            InventoryManager.ClearHand(hand);
        }

        public override bool TryAddPile(ItemPile pile)
        {
            return InventoryManager.TryAddItemPileToHand(pile, hand);
        }

        public void RemovePileFromHand(bool shouldDropItems)
        {
            HandsManager?.RemoveItemPileFromHand(hand, shouldDropItems: shouldDropItems);
        }
    }
}
