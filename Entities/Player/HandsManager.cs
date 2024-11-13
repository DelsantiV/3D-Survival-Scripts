using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandsManager
{
    private readonly GameObject rightHand;
    private readonly GameObject leftHand;
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


    public HandsManager(GameObject leftHand, GameObject rightHand, Hand prefHand)
    {
        this.leftHand = leftHand;
        this.rightHand = rightHand;
        this.PrefHand = prefHand;

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
            case Hand.both: return HandGO(PrefHand);
        }
        return null;
    }
    public Transform HandTransform(Hand hand)
    {
        switch (hand)
        {
            case Hand.left: return leftHand.transform;
            case Hand.right: return rightHand.transform;
            case Hand.both: return HandTransform(PrefHand);
        }
        return null;
    }

    public EquippedItem EquippedItemPileInHand(Hand hand)
    {
        return HandTransform(hand).gameObject.GetComponentInChildren<EquippedItem>();
    }

    public ItemPileInWorld EquippedPileInHand(Hand hand)
    {
        return HandTransform(hand).gameObject.GetComponentInChildren<ItemPileInWorld>();
    }

    public void AddItemToEquippedPileInHand(GeneralItem item, Hand hand)
    {
        EquippedPileInHand(hand).AddItem(item);
    }

    public void RemoveItemToEquippedPileInHand(GeneralItem item, Hand hand)
    {
        EquippedPileInHand(hand).RemoveItem(item);
    }

    public void RemoveItemToEquippedPileInHand(int index, Hand hand)
    {
        EquippedPileInHand(hand).RemoveItem(index);
    }

    public void ChangePileOfHand(Hand fromHand, Hand toHand)
    {
        EquippedItemPileInHand(fromHand).ChangeParent(HandTransform(toHand));
    }

    public void InstantiateItemPileInHand(ItemPile pile, Hand hand)
    {
        if (pile.NumberOfItemsInPile == 0) { return; }
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

    public void RemoveItemPileFromHand(Hand hand)
    {
        if (EquippedItemPileInHand(hand) != null)
        {
            Debug.Log("Removing physical pile from hand " + EquippedItemPileInHand(hand).name);
            EquippedItemPileInHand(hand).Remove();
        }
        else
        {
            Debug.Log("No item pile to remove from this hand !");
        }
    }

    public void DropItemPileFromHand(Hand hand)
    {
        if (EquippedItemPileInHand(hand) != null)
        {
            Debug.Log("Removing " + EquippedItemPileInHand(hand).name);
            EquippedItemPileInHand(hand).Drop();
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