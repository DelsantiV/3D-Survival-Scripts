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
        public Hand ActionHand;

        public HandMode CurrentHandMode { get; private set; }


        public HandsManager(GameObject leftHand, GameObject rightHand, Hand prefHand = Hand.right, GameObject bothHand = null)
        {
            this.leftHand = leftHand;
            this.rightHand = rightHand;
            this.PrefHand = prefHand;
            if (bothHand != null) this.bothHand = bothHand;
            else this.bothHand = HandGO(prefHand);

            CurrentHandMode = HandMode.single;
            ActionHand = Hand.none;
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

        public bool IsHandActive(Hand hand)
        {
            return ActiveHands.Contains(hand);
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

        public EquippedItemPile EquippedItemPileInHand(Hand hand)
        {
            Transform handTransform = HandTransform(hand);
            if (handTransform != null && IsHandActive(hand)) return handTransform.gameObject.GetComponentInChildren<EquippedItemPile>();
            else return null;
        }

        public int AnimationID(Hand hand)
        {
            EquippedItemPile itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) return EquippedItemPileInHand(hand).AnimationID;
            else return 0;
        }

        public void UseItemInHand(Hand hand)
        {
            EquippedItemPile itemInHand = EquippedItemPileInHand(hand);
            ActionHand = hand;
            if (itemInHand != null && hand != Hand.none) EquippedItemPileInHand(hand).Use();
        }
        public void StopUseItemInHand(Hand hand)
        {
            EquippedItemPile itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) EquippedItemPileInHand(hand).StopUse();
        }
        public void StopUseItemInHand()
        {
            if (ActionHand != Hand.none)
            {
                StopUseItemInHand(ActionHand);
                ActionHand = Hand.none;
            }
        }

        public void StartAction(Hand hand)
        {
            EquippedItemPile itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) EquippedItemPileInHand(hand).StartAction();
        }
        public void EndAction(Hand hand)
        {
            EquippedItemPile itemInHand = EquippedItemPileInHand(hand);
            if (itemInHand != null && hand != Hand.none) EquippedItemPileInHand(hand).EndAction();
        }
        public void EndAction()
        {
            if (ActionHand != Hand.none)
            {
                EndAction(ActionHand);
            }
        }
            public ItemPileInWorld EquippedPileInHand(Hand hand)
        {
            return HandTransform(hand).gameObject.GetComponentInChildren<ItemPileInWorld>();
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
            equippedPile.AddComponent<EquippedItemPile>();
        }

        public void ParentPileToHand(ItemPileInWorld pileInWorld, Hand hand)
        {
            if (pileInWorld != null)
            {
                pileInWorld.ChangeParent(HandTransform(hand));
                if (!pileInWorld.TryGetComponent<EquippedItemPile>(out var equippedPile)){ pileInWorld.AddComponent<EquippedItemPile>(); }
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
            EquippedItemPile equippedItem = EquippedItemPileInHand(hand);
            if (equippedItem != null)
            {
                Debug.Log("Removing physical pile from hand " + equippedItem.name);
                if (shouldDropItems) { equippedItem.Drop(); }
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