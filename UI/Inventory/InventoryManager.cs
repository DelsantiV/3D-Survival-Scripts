using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryManager
{
    private List<InventoryItemInfos> itemsInInventory;
    private GeneralInventoryUI inventoryUI;
    private GameObject iconInInventory;

    public int numberOfSlots;
    private GameObject slotGroup;
    private List<ItemSlot> slotList = new List<ItemSlot>();

    public void SetInventoryUI(GeneralInventoryUI inventoryUI)
    {
        this.inventoryUI = inventoryUI;
        inventoryUI.CreateInventoryUI(numberOfSlots);
        iconInInventory = Resources.Load<GameObject>("UI/IconInInventoryTemplate");
        slotGroup = inventoryUI.slotGroup;
        FindSlots();
        RefreshInventoryItemsinUI();
        Debug.Log(slotList.Count+ " slots in inventory");
    }
    
    public InventoryManager(int numberOfSlots, GeneralInventoryUI inventoryUI)
    {
        itemsInInventory = new List<InventoryItemInfos>();
        this.numberOfSlots = numberOfSlots;
        SetInventoryUI(inventoryUI);
    }

    public void AddNewItemToInventoryEmptySlot(Item_General_SO itemSO, int amount)
    {
        ItemSlot slotToEquip = FindNextEmptySlot();
        slotToEquip.AddItemToNewSlot(iconInInventory, itemSO, amount);
        
        itemsInInventory.Add(new InventoryItemInfos(itemSO, amount));
    }

    public void AddItemToInventory(Item_General_SO itemSO, int amount)
    {
        int amountLeft = amount;
        int maxStackSize = itemSO.maxStackSize;
        if (IsItemInInventory(itemSO))
        {
            ItemSlot slotWithItem = FindNextSlotkWithItemNotFull(itemSO);
            while (slotWithItem != null && amountLeft > 0)
            {
                int currentAmount = slotWithItem.currentItem.GetComponent<ItemInInventory>().amountOfItem;
                int amountToAdd = Mathf.Min(maxStackSize - currentAmount, amountLeft);
                slotWithItem.AddItemAmountToSlot(amountToAdd);
                amountLeft -= amountToAdd;
                slotWithItem = FindNextSlotkWithItemNotFull(itemSO);

            }
        }


        ItemSlot slotEmpty = FindNextEmptySlot();
        while (slotEmpty != null && amountLeft > 0)
        {
            int amountToAdd = Mathf.Min(maxStackSize, amountLeft);
            slotEmpty.AddItemToNewSlot(iconInInventory, itemSO, amountToAdd);
            amountLeft -= amountToAdd;
            slotEmpty = FindNextEmptySlot();
        }

        if (amountLeft > 0)
        {
            Debug.Log("Could not add item, inventory is full");
            PlayerManager.Player.SpawnItemFromPlayer(itemSO, amountLeft);
        }

        if (amountLeft < amount)
        {
            InventoryItemInfos item = FindItemInInventory(itemSO);
            if (item != null)
            {
                item.itemAmount += amount - amountLeft;
            }
            else
            {
                itemsInInventory.Add(new InventoryItemInfos(itemSO, amount - amountLeft));
            }
        }

    }

    public void RemoveItemFromInventory(Item_General_SO itemSO, int amount) 
    {
        ItemSlot slotWithItem = FindNextSlotkWithItem(itemSO);
        int amountLeft = amount;
        while (slotWithItem != null && amountLeft > 0)
        {
            int currentAmount = slotWithItem.currentItem.GetComponent<ItemInInventory>().amountOfItem;
            int amountToRemove = Mathf.Min(currentAmount, amountLeft);
            slotWithItem.RemoveItemAmountFromSlot(amountToRemove);
            amountLeft -= amountToRemove;
            //Debug.Log("Removing " + amountToRemove + itemSO.itemName + " from " + slotWithItem.name + " (Containing "+currentAmount+"). Still "+amountLeft+" to remove.");
            slotWithItem = FindNextSlotkWithItem(itemSO);

        }

        RemoveItemFromInventoryList(itemSO, amount - amountLeft);
        if (amountLeft > 0)
        {
            Debug.Log("Could not remove enough items, no more in inventory");
        }
    }

    public void RemoveItemFromInventory(ItemSlot slotWithItem, int amount)
    {
        int currentAmount = slotWithItem.currentItem.amountOfItem;
        if  (currentAmount < amount)
        {
            Debug.Log("Tried to remove too many items from slot, capped to current amount.");
            amount = currentAmount;
        }
        RemoveItemFromInventoryList(slotWithItem.currentItem.itemSO, amount);
        slotWithItem.RemoveItemAmountFromSlot(amount);
    }

    public List<InventoryItemInfos> GetItemsInInventory()
    { 
        return itemsInInventory;
    }

    public void RefreshItemsInInventory()
    {
        foreach (ItemSlot slot in slotList)
        {
            if (slot.currentItem != null)
            {
                AddItemToInventoryList(slot.currentItem.itemSO, slot.currentItem.amountOfItem);
            }
        }
    }

    public void RefreshInventoryItemsinUI()     
    {
        if (itemsInInventory != null)
        {
            foreach (InventoryItemInfos item in itemsInInventory)
            {
                AddItemToInventory(item.itemSO, item.itemAmount);
            }
        }
    }

    public bool IsItemInInventory(Item_General_SO itemSO)
    {
        foreach (InventoryItemInfos item in itemsInInventory)    
        {
            if (item.itemSO == itemSO) { return true; }
        }
        return false;
    }

    public int AmountOfItemInInventory(Item_General_SO itemSO)
    {
        InventoryItemInfos item = FindItemInInventory(itemSO);
        if (item == null) return 0;
        else return item.itemAmount;
    }

    public InventoryItemInfos FindItemInInventory(Item_General_SO itemSO)
    {
        foreach (InventoryItemInfos item in itemsInInventory)
        {
            if (item.itemSO == itemSO) { return item; }
        }
        return null;
    }

    private void AddItemToInventoryList(Item_General_SO itemSO, int amount)
    {
        if (amount > 0)
        {
            InventoryItemInfos item = FindItemInInventory(itemSO);
            if (item != null)
            {
                item.itemAmount += amount;
            }
            else
            {
                itemsInInventory.Add(new InventoryItemInfos(itemSO, amount));
            }
        }
    }

    private void RemoveItemFromInventoryList(Item_General_SO itemSO, int amount)
    {
        InventoryItemInfos item = FindItemInInventory(itemSO);
        if (item != null)
        {
            item.itemAmount -= amount;
            if (item.itemAmount <= 0)
            {
                itemsInInventory.Remove(item);
            }
        }
    }

    public void FindSlots()
    {
        foreach (Transform slot in slotGroup.transform)
        {
            slotList.Add(slot.GetComponent<ItemSlot>());
        }
    }

    public ItemSlot FindNextEmptySlot()     
    {
        foreach (ItemSlot slot in slotList)
        {
            if (slot.currentItem == null)
            {
                return slot;
            }
        }
        return null;
    }

    public ItemSlot FindNextSlotkWithItem(Item_General_SO itemSO)
    {
        foreach (ItemSlot slot in slotList)
        {
            if (slot.currentItem != null)
            {
                if (slot.currentItem.itemSO == itemSO && slot.currentItem.amountOfItem > 0)
                {
                    return slot;
                }
            }
        }
        return null;
    }

    public ItemSlot FindNextSlotkWithItemNotFull(Item_General_SO itemSO)
    {
        foreach (ItemSlot slot in slotList)
        {
            if (slot.currentItem != null)
            {
                if (slot.currentItem.itemSO == itemSO && slot.currentItem.amountOfItem < itemSO.maxStackSize)
                {
                    return slot;
                }
            }
        }
        return null;
    }
}
