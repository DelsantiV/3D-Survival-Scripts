using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandsManager
{
    private GameObject rightHand;
    private GameObject leftHand;
    private QuickSlot leftHandQuickSlot;
    private QuickSlot rightHandQuickSlot;
    private Hand prefHand = Hand.right;
    private Hand otherHand;


    public HandsManager(GameObject leftHand, GameObject rightHand, QuickSlot leftHandQuickSlot, QuickSlot rightHandQuickSlot, Hand prefHand)
    {
        this.leftHand = leftHand;
        this.rightHand = rightHand;
        this.leftHandQuickSlot = leftHandQuickSlot;
        this.rightHandQuickSlot = rightHandQuickSlot;
        this.prefHand = prefHand;
        otherHand = GetOtherHand(prefHand);
        leftHandQuickSlot.SetHandsManager(this, Hand.left);
        rightHandQuickSlot.SetHandsManager(this, Hand.right);
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

    public GameObject HandGO(Hand hand)
    {
        switch (hand)
        {
            case Hand.left: return leftHand;
            case Hand.right: return rightHand;
            case Hand.both: return HandGO(prefHand);
        }
        return null;
    }
    public Transform HandTransform(Hand hand)
    {
        switch (hand)
        {
            case Hand.left: return leftHand.transform;
            case Hand.right: return rightHand.transform;
            case Hand.both: return HandTransform(prefHand);
        }
        return null;
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

    private Hand GetOtherHand(Hand prefHand)
    {
        switch (prefHand)
        {
            case Hand.left: return Hand.right;
            case Hand.right: return Hand.left;
        }
        return Hand.left;
    }

    public ItemPileInInventory ItemPileUIInHand(Hand hand)
    {
        return HandQuickSlot(hand).CurrentPileUI;
    }

    public ItemPile ItemPileInHand(Hand hand)
    {
        return ItemPileUIInHand(hand).ItemPile;
    }

    public EquippedItem EquippedItemPileInHand(Hand hand)
    {
        return HandTransform(hand).GetComponentInChildren<EquippedItem>();
    }


    public void InstantiateItemPileInHand(ItemPile pile, Hand hand)
    {
        ItemPileInWorld equippedPile = pile.SpawnInWorld(HandTransform(hand));
        equippedPile.AddComponent<EquippedItem>();
    }

    public void RemoveItemPileFromHand(Hand hand)
    {
        if (!isHandEmpty(hand))
        {
            Debug.Log("Removing " + EquippedItemPileInHand(hand).name);
            EquippedItemPileInHand(hand).Remove();
        }
        else
        {
            Debug.Log("No item to remove from this hand !");
        }
    }

    private void EquipItemPileToHand(Hand hand, ItemPile pile)
    {
        HandQuickSlot(hand).AddPile(pile);
    }
    public bool isHandEmpty(Hand hand)
    {
        return HandQuickSlot(hand).IsEmpty;
    }

    public Hand GetNextEmptyHand()
    {
        if (isHandEmpty(prefHand)) { return prefHand; }
        else if (isHandEmpty(otherHand)) { return otherHand; }
        return Hand.none;
    }

    public bool TryEquipItemPileToNextEmptyHand(ItemPile pile)
    {
        Hand emptyHand = GetNextEmptyHand();
        if (emptyHand == Hand.none) { return false; }
        else
        {
            EquipItemPileToHand(emptyHand, pile);
            return true;
        }
    }
    public bool TryEquipItemPileToHand(ItemPile pile, Hand hand)
    {
        if (!isHandEmpty(hand)) { return false; }
        else
        {
            EquipItemPileToHand(hand, pile);
            return true;
        }
    }
}

