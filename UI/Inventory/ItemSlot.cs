using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;



public class ItemSlot : MonoBehaviour, IDropHandler
{

    [HideInInspector] public PlayerManager player = PlayerManager.Player;
    public ItemInInventory currentItem { get; private set; }


    public virtual int amountOfItem
    {
        get
        {
            if (currentItem != null)
            {
                return currentItem.amountOfItem;
            }
            return 0;
        }
    }


    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject itemBeingDragGO = eventData.pointerDrag;
        //GetComponent<Image>().color = Color.red;
        if (itemBeingDragGO != null)
        {
            ItemInInventory itemBeingDrag = itemBeingDragGO.GetComponent<ItemInInventory>();
            AddItem(itemBeingDrag);
        }

    }

    public virtual void AddItem(ItemInInventory item)
    {
        //if there is not item already then set our item.
        if (currentItem == null)
        {
            item.transform.SetParent(transform);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            currentItem = item;
        }

        //if there is an item already, check for stackability
        else
        {
            if (currentItem.itemSO == item.itemSO)
            {
                if (currentItem.itemSO.maxStackSize >= currentItem.amountOfItem + item.amountOfItem)
                {
                    currentItem.AddAmountOfItem(item.amountOfItem);
                    Destroy(item.gameObject);
                }
                else
                {
                    int amountToTransfer = currentItem.itemSO.maxStackSize - currentItem.amountOfItem;
                    currentItem.AddAmountOfItem(amountToTransfer);
                    item.RemoveAmountOfItem(amountToTransfer);
                }
            }
            else
            {
                Debug.Log("Could not add item to slot, as different item was already in slot");
            }
        }
    }

    public void AddItemToNewSlot(GameObject iconInInventory, Item_General_SO itemSO, int amount)
    {
        if (currentItem == null)
        {
            GameObject itemInInventoryGO = Instantiate(iconInInventory, transform.position, transform.rotation);
            itemInInventoryGO.transform.SetParent(transform);
            itemInInventoryGO.AddComponent(Type.GetType(itemSO.item_class));
            ItemInInventory itemInInventory = itemInInventoryGO.GetComponent<ItemInInventory>();
            itemInInventory.CreateItemInInventory(itemSO, amount);
            currentItem = itemInInventory;
        }
        else { Debug.Log("Could not add item cause slot was not empty !"); }
    }

    //Could be fused with previous method, kept apart for now
    public void AddItemAmountToSlot(int amount)
    {
        if (currentItem != null)
        {
            currentItem.AddAmountOfItem(amount);
        }

    }

    public virtual void RemoveItemAmountFromSlot(int amount)
    {
        if (currentItem != null)
        {
            if (currentItem.amountOfItem < amount)
            {
                Debug.Log("Not enough items in slot !");
                return;
            }
            currentItem.RemoveAmountOfItem(amount);
            
            if (currentItem.amountOfItem == 0)
            {
                DestroyItem();
            }
        }
        else { Debug.Log("Problemos"); }
    }

    public virtual void DestroyItem()
    {
        currentItem.CloseItemInfo();
        Destroy(currentItem.gameObject);
        currentItem = null;
    }
    public virtual void RemoveItem()
    {
        if (currentItem != null)
        {
            currentItem.CloseItemInfo();
            currentItem = null;
        }
    }
}

