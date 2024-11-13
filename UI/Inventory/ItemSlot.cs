using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;



public class ItemSlot : MonoBehaviour, IDropHandler
{

    [HideInInspector] public PlayerManager Player {get; private set;}

    public ItemPileInInventory CurrentPileUI 
    { 
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).GetComponent<ItemPileInInventory>();
            }

            return null;
        }
    }
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
            if (CurrentPileUI == null)
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

    public void SetPlayer(PlayerManager player)
    {
        Player = player;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject itemBeingDragGO = eventData.pointerDrag;
        if (itemBeingDragGO != null)
        {
            ItemPileInInventory itemBeingDrag = itemBeingDragGO.GetComponent<ItemPileInInventory>();
            if (itemBeingDrag.slot != this)
            {
                itemBeingDrag.ChangeSlot(this);
            }
        }
    }

    public virtual void AddPile(ItemPile pile)
    {
        if (pile != null)
        {
            //if there is no pile already in the slot then set our pile.
            if (IsEmpty)
            {
                Debug.Log("Trying to add pile " + pile + " to slot " + name + ", which is empty");
                pile.SetItemPileToSlot(this);
            }

            //if there is a pile already, try to merge pile
            else
            {
                Debug.Log("Trying to add item " + pile + " to slot " + name + ", containing " + CurrentPile.ToString());
            }
        }
        else { Debug.Log("Trying to add null item"); }
    }

    public virtual void AddPile(ItemPileInInventory pileUI)
    {
        if (pileUI != null)
        {
            //if there is no pile already in the slot then set our pile.
            if (IsEmpty)
            {
                Debug.Log("Trying to add pile " + pileUI.ItemPile.ToString() + " to slot " + name + ", which is empty");
                SetItemPileToSlot(pileUI);
            }

            //if there is a pile already, try to merge pile
            else
            {
                Debug.Log("Trying to add item " + pileUI.ItemPile.ToString() + " to slot " + name + ", containing " + CurrentPile.ToString());
            }
        }
        else { Debug.Log("Trying to add null item"); }
    }

    public virtual void DestroyItem()
    {
        Debug.Log("Destroying pile " + CurrentPile.ToString() + " from slot " + name);
        Destroy(CurrentPileUI.gameObject);
    }
    public virtual void RemovePile(bool shouldDestroy = true)
    {
        if (CurrentPileUI != null)
        {
            Debug.Log("Removing item " + CurrentPile.ToString() + " from slot" + name);
            CurrentPileUI.CloseItemInfo();
            if (shouldDestroy) { DestroyItem(); }
        }
        else
        {
            Debug.Log("Tried removing item from slot " + name + ", but this slot was empty !");
        }
    }

    public virtual void SetItemPileToSlot(ItemPileInInventory pileUI)
    {
        pileUI.transform.SetParent(transform, false);
        pileUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        pileUI.RefreshSlot();
    }
}