using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GoTF.Content
{
    public class HandsManager
    {
        private readonly GameObject rightHand;
        private readonly GameObject leftHand;
        private readonly GameObject bothHand;
        public Hand PrefHand;

        public Hand OtherHand
        {
            get
            {
                return PrefHand switch
                {
                    Hand.right => Hand.left,
                    Hand.left => Hand.right,
                    _ => Hand.none,
                };
            }
        }

        public HandMode CurrentHandMode { get; private set; }


        public HandsManager(GameObject leftHand, GameObject rightHand, Hand prefHand = Hand.right, GameObject bothHand = null)
        {
            this.leftHand = leftHand;
            this.rightHand = rightHand;
            this.PrefHand = prefHand;
            if (bothHand != null) this.bothHand = bothHand;
            else this.bothHand = HandGO(prefHand);

            CurrentHandMode = HandMode.single;
        }

        public enum Hand
        {
            right,
            left,
            both,
            none
        }

        public enum HandMode
        {
            both,
            single,
            none
        }

        public List<Hand> ActiveHands
        {
            get
            {
                return CurrentHandMode switch
                {
                    (HandMode.both) => new List<Hand>() { Hand.both },
                    (HandMode.single) => new List<Hand>() { PrefHand, OtherHand },
                    _ => new List<Hand>(),
                };
            }
        }

        public GameObject HandGO(Hand hand)
        {
            switch (hand)
            {
                case Hand.left: return leftHand;
                case Hand.right: return rightHand;
                case Hand.both: return bothHand;
            }
            return null;
        }
        public Transform HandTransform(Hand hand)
        {
            switch (hand)
            {
                case Hand.left: return leftHand.transform;
                case Hand.right: return rightHand.transform;
                case Hand.both: return bothHand.transform;
            }
            return null;
        }

        public EquippedItem EquippedItemPileInHand(Hand hand)
        {
            return HandTransform(hand).gameObject.GetComponentInChildren<EquippedItem>();
        }

        public int AnimationID(Hand hand)
        {
            EquippedItem itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) return EquippedItemPileInHand(hand).AnimationID;
            else return 0;
        }

        public void UseItemInHand(Hand hand)
        {
            EquippedItem itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) EquippedItemPileInHand(hand).Use();
        }
        public void StopUseItemInHand(Hand hand)
        {
            EquippedItem itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) EquippedItemPileInHand(hand).StopUse();
        }

        public void StartAction(Hand hand)
        {
            EquippedItem itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) EquippedItemPileInHand(hand).StartAction();
        }
        public void EndAction(Hand hand)
        {
            EquippedItem itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) EquippedItemPileInHand(hand).EndAction();
        }

        public ItemPileInWorld EquippedPileInHand(Hand hand)
        {
            return HandTransform(hand).gameObject.GetComponentInChildren<ItemPileInWorld>();
        }

        public void AddItemToEquippedPileInHand(GeneralItem item, Hand hand)
        {
            if (hand != Hand.none && item != null) EquippedPileInHand(hand).AddItem(item);
        }

        public void RemoveItemToEquippedPileInHand(GeneralItem item, Hand hand)
        {
            if (hand != Hand.none && item != null) EquippedPileInHand(hand).RemoveItem(item);
        }

        public void RemoveItemToEquippedPileInHand(int index, Hand hand)
        {
            EquippedPileInHand(hand).RemoveItem(index);
        }

        public void ChangePileOfHand(Hand fromHand, Hand toHand)
        {
            if (fromHand != Hand.none && toHand != Hand.none) EquippedItemPileInHand(fromHand).ChangeParent(HandTransform(toHand));
        }

        public void InstantiateItemPileInHand(ItemPile pile, Hand hand)
        {
            if (pile.NumberOfItemsInPile == 0 || hand == Hand.none) { return; }
            ItemPileInWorld equippedPile = pile.SpawnInHands(HandTransform(hand));
            equippedPile.AddComponent<EquippedItem>();
        }

        public void ParentPileToHand(ItemPileInWorld pileInWorld, Hand hand)
        {
            if (pileInWorld != null)
            {
                pileInWorld.ChangeParent(HandTransform(hand));
            }
        }
        public void ParentPileToHand(ItemPile pile, Hand hand)
        {
            if (pile.IsInWorld)
            {
                ParentPileToHand(pile.ItemPileInWorld, hand);
            }
        }

        public void RemoveItemPileFromHand(Hand hand, bool shouldDropItems = false)
        {
            EquippedItem equippedItem = EquippedItemPileInHand(hand);
            if (equippedItem != null)
            {
                Debug.Log("Removing physical pile from hand " + equippedItem.name);
                if (shouldDropItems) { equippedItem .Drop(); }
                else { equippedItem.Remove(); }
            }
            else
            {
                Debug.Log("No item pile to remove from this hand !");
            }
        }

        public void SetHandModes(HandMode handMode)
        {
            if (CurrentHandMode == handMode) { return; }

            if (CurrentHandMode == HandMode.single)
            {

            }
            CurrentHandMode = handMode;
        }
    }
}