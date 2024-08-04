using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlot : ItemSlot
{
    [HideInInspector] public static QuickSlot activeQuickSlot;
    private HandsManager handsManager;
    public HandsManager.Hand hand;


    public virtual void SetHandsManager(HandsManager handsManager,  HandsManager.Hand hand)
    {
        this.hand = hand;
        this.handsManager = handsManager;
    }

    public override void AddItem(ItemInInventory item)
    {
        if (currentItem == null)
        {
            handsManager.InstantiateItemInHand(item, hand);
        }
        base.AddItem(item);
        // Needs to be improved: do not instantiate item if could not add
    }
    public override void AddItem(GeneralItem item, int amount = 1)
    {
        if (currentItem == null)
        {
            handsManager.InstantiateItemInHand(item, hand);
        }
        base.AddItem(item);
        // Needs to be improved: do not instantiate item if could not add
    }

    public override void OnDrop(PointerEventData eventData)
    {
        GameObject itemBeingDragGO = eventData.pointerDrag;
        ItemInInventory item;
        if (itemBeingDragGO != null && itemBeingDragGO.TryGetComponent(out item)) 
        { 
            AddItem(item);
        }
    }

    public void RemoveItemFromHands()
    {
        handsManager.RemoveItemFromHand(hand); 
    }
}
