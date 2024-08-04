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
    public ItemInInventory currentItemUI { get; private set; }
    public bool _isInOp = false;
    public GeneralItem currentItem 
    {  
        get
        { 
            if (currentItemUI == null)
            {
                return null;
            }
            else
            {
                return currentItemUI.Item;
            }
        }
    }


    public virtual int amountOfItem
    {
        get
        {
            if (currentItemUI != null)
            {
                return currentItemUI.amountOfItem;
            }
            return 0;
        }
    }

    public virtual bool isEmpty
    {
        get
        {
            if (currentItemUI == null)
            {
                return true;
            }
            return false;
        }
        set 
        { 
            if (currentItem == null && !_isInOp) 
            {
                isEmpty = true;
            }
            else
            {
                isEmpty = false;
            }
            return;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject itemBeingDragGO = eventData.pointerDrag;
        if (itemBeingDragGO != null)
        {
            ItemInInventory itemBeingDrag = itemBeingDragGO.GetComponent<ItemInInventory>();
            if (itemBeingDrag.slot != this)
            {
                AddItem(itemBeingDrag.Item, itemBeingDrag.amountOfItem);
            }
        }

    }

    public virtual void RefreshItem(bool manual)
    {
        currentItemUI = transform.GetComponentInChildren<ItemInInventory>(true);
    }

    public virtual void AddItem(GeneralItem item, int amount = 1)
    {
        if (item != null)
        {
            //if there is not item already then set our item.
            if (isEmpty)
            {
                Debug.Log("Trying to add item " + item.ItemName + " to slot " + name + ", which is empty");
                Task itemUICreation = new Task(item.CreateItemInInventory(amount, this));
                itemUICreation.Finished += RefreshItem;
            }

            //if there is an item already, check for stackability
            else
            {
                Debug.Log("Trying to add item " + item.ItemSO + " to slot " + name + ", containing " + currentItem.ItemSO);
                if (currentItem.ItemSO == item.ItemSO)
                {
                    if (currentItem.ItemSO.maxStackSize >= currentItemUI.amountOfItem + amount)
                    {
                        currentItemUI.AddAmountOfItem(amount);
                    }
                    else
                    {
                        int amountToTransfer = currentItem.ItemSO.maxStackSize - currentItemUI.amountOfItem;
                        currentItemUI.AddAmountOfItem(amountToTransfer);
                    }
                }
                else
                {
                    Debug.Log("Could not add item to slot, as different item was already in slot");
                }
            }
        }
        else { Debug.Log("Trying to add null item"); }
    }

    public virtual void AddItem(ItemInInventory itemUI)
    {
        if (itemUI != null)
        {
            //if there is not item already then set our item.
            if (isEmpty)
            {
                // Reparent itemUI to this slot
                SetItemToSlot(itemUI);
            }

            //if there is an item already, check for stackability
            else
            {
                Debug.Log("Trying to add item " + itemUI.ItemSO + " to slot " + name + ", containing " + currentItem.ItemSO);
                if (currentItemUI.ItemSO == itemUI.ItemSO)
                {
                    if (currentItem.ItemSO.maxStackSize >= currentItemUI.amountOfItem + itemUI.amountOfItem)
                    {
                        currentItemUI.AddAmountOfItem(itemUI.amountOfItem);
                    }
                    else
                    {
                        int amountToTransfer = currentItem.ItemSO.maxStackSize - currentItemUI.amountOfItem;
                        currentItemUI.AddAmountOfItem(amountToTransfer);
                        itemUI.RemoveAmountOfItem(amountToTransfer);
                    }
                }
                else
                {
                    Debug.Log("Could not add item to slot, as different item was already in slot");
                }
            }
        }
        else { Debug.Log("Trying to add null item"); }
    }


    public void AddItemToNewSlot(GameObject iconInInventory, GeneralItem item, int amount)
    {
        if (currentItem == null)
        {
            GameObject itemInInventoryGO = Instantiate(iconInInventory, transform.position, transform.rotation);
            itemInInventoryGO.transform.SetParent(transform);
            ItemInInventory itemInInventory = itemInInventoryGO.GetComponent<ItemInInventory>();
            itemInInventory.CreateItemInInventory(item, amount); //Needs to be modified
            currentItemUI = itemInInventory;
        }
        else { Debug.Log("Could not add item cause slot was not empty !"); }
    }

    //Could be fused with previous method, kept apart for now
    public void AddItemAmountToSlot(int amount)
    {
        if (currentItemUI != null)
        {
            currentItemUI.AddAmountOfItem(amount);
        }

    }

    public virtual void RemoveItemAmountFromSlot(int amount)
    {
        if (currentItemUI != null)
        {
            if (currentItemUI.amountOfItem < amount)
            {
                Debug.Log("Not enough items in slot !");
                return;
            }
            currentItemUI.RemoveAmountOfItem(amount);

            if (currentItemUI.amountOfItem == 0)
            {
                DestroyItem();
            }
        }
        else { Debug.Log("Problemos"); }
    }

    public virtual void DestroyItem()
    {
        Debug.Log("Destroying item " + currentItem.ItemSO + " from slot " + name);
        currentItemUI.CloseItemInfo();
        Destroy(currentItemUI.gameObject);
        currentItemUI = null;
    }
    public virtual void RemoveItem()
    {
        if (currentItem != null)
        {
            Debug.Log("Removing item " + currentItem.ItemSO + " from slot" + name);
            currentItemUI.CloseItemInfo();
            currentItemUI = null;
        }
        else
        {
            Debug.Log("Tried removing item from slot " + name + ", but this slot was empty !");
        }
    }

    public virtual void SetItemToSlot(ItemInInventory itemUI)
    {
        currentItemUI = itemUI;
        itemUI.transform.SetParent(transform);
        itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        itemUI.RefreshSlot();
    }
}