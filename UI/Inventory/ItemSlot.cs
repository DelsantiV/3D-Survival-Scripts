using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using static UnityEditor.Progress;



public class ItemSlot : MonoBehaviour, IDropHandler
{

    [HideInInspector] public PlayerManager player = PlayerManager.Player;
    public bool _isInOp = false;

    public ItemPileInInventory CurrentPileUI { get; private set; }
    public ItemPile CurrentPile
    {
        get
        {
            if (CurrentPileUI == null) { return null; }
            else { return CurrentPileUI.ItemPile; }
        }
    }


    public virtual bool IsEmpty
    {
        get
        {
            if (CurrentPileUI == null && !_isInOp)
            {
                return true;
            }
            return false;
        }
    }

    public virtual bool IsHoldingSingleItem
    {
        get { return (CurrentPileUI.ItemPile.IsPileUniqueItem); }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject itemBeingDragGO = eventData.pointerDrag;
        if (itemBeingDragGO != null)
        {
            ItemPileInInventory itemBeingDrag = itemBeingDragGO.GetComponent<ItemPileInInventory>();
            if (itemBeingDrag.slot != this)
            {
                AddPile(itemBeingDrag.ItemPile);
            }
        }

    }

    public virtual void RefreshItemPile(bool manual)
    {
        CurrentPileUI = transform.GetComponentInChildren<ItemPileInInventory>(true);
    }

    public virtual void AddPile(ItemPile pile)
    {
        if (pile != null)
        {
            //if there is no pile already in the slot then set our pile.
            if (IsEmpty)
            {
                Debug.Log("Trying to add pile " + pile + " to slot " + name + ", which is empty");
                pile.SetItemToSlot(this);
            }

            //if there is a pile already, try to merge pile
            else
            {
                Debug.Log("Trying to add item " + pile + " to slot " + name + ", containing " + CurrentPile.ToString());
            }
        }
        else { Debug.Log("Trying to add null item"); }
    }

    public virtual void DestroyItem()
    {
        Debug.Log("Destroying pile " + CurrentPile.ToString() + " from slot " + name);
        Destroy(CurrentPileUI.gameObject);
        CurrentPileUI = null;
    }
    public virtual void RemovePile()
    {
        if (CurrentPileUI != null)
        {
            Debug.Log("Removing item " + CurrentPile.ToString() + " from slot" + name);
            CurrentPileUI.CloseItemInfo();
            CurrentPileUI = null;
        }
        else
        {
            Debug.Log("Tried removing item from slot " + name + ", but this slot was empty !");
        }
    }

    public virtual void SetItemPileToSlot(ItemPileInInventory pileUI)
    {
        CurrentPileUI = pileUI;
        pileUI.transform.SetParent(transform);
        pileUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        pileUI.RefreshSlot();
    }
}