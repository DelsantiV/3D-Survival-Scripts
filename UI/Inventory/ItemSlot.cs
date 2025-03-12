using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;


namespace GoTF.Content
{
    public class ItemSlot : MonoBehaviour, IDropHandler
    {

        [HideInInspector] public PlayerManager Player { get; private set; }

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
        public virtual ItemPile CurrentPile
        {
            get
            {
                if (CurrentPileUI == null) { return new(); }
                else { return CurrentPileUI.ItemPile; }
            }
        }


        public virtual bool IsEmpty
        {
            get
            {

                if (CurrentPile.IsEmpty) return true;
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
                if (itemBeingDrag.slot != null)
                {
                    if (itemBeingDrag.slot != this)
                    {
                        ItemSlot previousSlot = itemBeingDrag.slot;
                        if (TryAddPile(itemBeingDrag.ItemPile))
                        {
                            previousSlot.RemovePile(false, false);
                            Debug.Log("Was able to add pile to existing one");
                        }
                        else
                        {
                            
                        }
                        
                    }
                }
            }
        }

        public virtual bool TryAddPile(ItemPile pile)
        {
            return true;
        }

        public virtual void DestroyItem()
        {
            Debug.Log("Destroying pile " + CurrentPile.ToString() + " from slot " + name);
            Destroy(CurrentPileUI.gameObject);
        }
        public virtual void RemovePile(bool shouldDestroy = true, bool shouldDropItems = false)
        {
            if (CurrentPileUI != null)
            {
                Debug.Log("Removing item " + CurrentPile.ToString() + " from slot" + name);
                CurrentPileUI.ClosePileInfo();
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
}