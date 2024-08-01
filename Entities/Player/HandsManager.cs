using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsManager
{
    private GameObject rightHand;
    private GameObject leftHand;
    private QuickSlot leftHandQuickSlot;
    private QuickSlot rightHandQuickSlot;
    private Hand prefHand = Hand.right;
    private Hand otherHand;


    public HandsManager(GameObject leftHand, GameObject rightHand, QuickSlot leftHandQuickSlot, QuickSlot rightHandQuickSlot, HandsManager.Hand prefHand)
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

    public ItemInInventory ItemInHand(Hand hand)
    {
        return HandQuickSlot(hand).currentItem;
    }

    public EquippedItem EquippedItemInHand(Hand hand)
    {
        return HandTransform(hand).GetComponentInChildren<EquippedItem>();
    }

    public void InstantiateItemInHand(ItemInInventory item, Hand hand)
    {
        GameObject itemInHand = GameObject.Instantiate(item.ItemGO(), HandTransform(hand));
        itemInHand.GetComponent<BoxCollider>().isTrigger = true;  // à améliorer
        itemInHand.AddComponent<EquippedItem>();
    }

    public void RemoveItemFromHand(Hand hand)
    {
        if (!isHandEmpty(hand))
        {
            Debug.Log("Removing " + EquippedItemInHand(hand).name);
            EquippedItemInHand(hand).Remove();
        }
        else
        {
            Debug.Log("No item to remove from this hand !");
        }
    }

    private void EquipItemToHand(Hand hand, ItemInInventory item)
    {
        HandQuickSlot(hand).AddItem(item);
    }
    private void EquipItemToHand(Hand hand, ItemInInventory item, Vector3 itemPosition, Quaternion itemRotation)
    {
        GameObject itemInHand = GameObject.Instantiate(item.ItemGO(), itemPosition, itemRotation, HandTransform(hand));
    }

    public bool isHandEmpty(Hand hand)
    {
        return HandQuickSlot(hand).isEmpty;
    }

    public Hand GetNextEmptyHand()
    {
        if (isHandEmpty(prefHand)) { return prefHand; }
        else if (isHandEmpty(otherHand)) { return otherHand; }
        return Hand.none;
    }

    public bool TryEquipItemToNextEmptyHand(ItemInInventory item)
    {
        Hand emptyHand = GetNextEmptyHand();
        if (emptyHand == Hand.none) { return false; }
        else
        {
            EquipItemToHand(emptyHand, item);
            return true;
        }
    }

    public bool TryEquipItemToNextEmptyHand(ItemInInventory item, Vector3 itemPosition, Quaternion itemRotation)
    {
        Hand emptyHand = GetNextEmptyHand();
        if (emptyHand == Hand.none) { return false; }
        else
        {
            EquipItemToHand(emptyHand, item, itemPosition, itemRotation);
            return true;
        }
    }

    public bool TryEquipItemToHand(ItemInInventory item, Hand hand)
    {
        if (!isHandEmpty(hand)) { return false; }
        else
        {
            EquipItemToHand(hand, item);
            return true;
        }
    }


    public bool TryEquipItemToHand(ItemInInventory item, Vector3 itemPosition, Quaternion itemRotation, Hand hand)
    {
        if (!isHandEmpty(hand)) { return false; }
        else
        {
            EquipItemToHand(hand, item, itemPosition, itemRotation);
            return true;
        }
    }
}

