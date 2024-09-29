using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandsManager
{
    private readonly GameObject rightHand;
    private readonly GameObject leftHand;
    private Hand prefHand;

    public HandMode CurrentHandMode { get; private set; }


    public HandsManager(GameObject leftHand, GameObject rightHand, Hand prefHand)
    {
        this.leftHand = leftHand;
        this.rightHand = rightHand;
        this.prefHand = prefHand;

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

    public EquippedItem EquippedItemPileInHand(Hand hand)
    {
        return HandTransform(hand).gameObject.GetComponentInChildren<EquippedItem>();
    }

    public ItemPileInWorld EqquippedPileInHand(Hand hand)
    {
        return HandTransform(hand).gameObject.GetComponentInChildren<ItemPileInWorld>();
    }

    public void AddItemToEquippedPileInHand(GeneralItem item, Hand hand)
    {
        EqquippedPileInHand(hand).AddItem(item);
    }

    public void RemoveItemToEquippedPileInHand(GeneralItem item, Hand hand)
    {
        EqquippedPileInHand(hand).RemoveItem(item);
    }

    public void RemoveItemToEquippedPileInHand(int index, Hand hand)
    {
        EqquippedPileInHand(hand).RemoveItem(index);
    }

    public void InstantiateItemPileInHand(ItemPile pile, Hand hand)
    {
        ItemPileInWorld equippedPile = pile.SpawnInHands(HandTransform(hand));
        equippedPile.AddComponent<EquippedItem>();
    }

    public void RemoveItemPileFromHand(Hand hand)
    {
        if (EquippedItemPileInHand(hand) != null)
        {
            Debug.Log("Removing " + EquippedItemPileInHand(hand).name);
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