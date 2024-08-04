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

    public void AddNewItemToInventoryEmptySlot(GeneralItem item, int amount)
    {
        ItemSlot slotToEquip = FindNextEmptySlot();
        slotToEquip.AddItemToNewSlot(iconInInventory, item, amount);
        
        itemsInInventory.Add(new InventoryItemInfos(item, amount));
    }

    public void AddItemToInventory(GeneralItem item, int amount)
    {
        int amountLeft = amount;
        int maxStackSize = item.StackSize;
        if (IsItemInInventory(item))
        {
            ItemSlot slotWithItem = FindNextSlotkWithItemNotFull(item);
            while (slotWithItem != null && amountLeft > 0)
            {
                int currentAmount = slotWithItem.currentItemUI.GetComponent<ItemInInventory>().amountOfItem;
                int amountToAdd = Mathf.Min(maxStackSize - currentAmount, amountLeft);
                slotWithItem.AddItemAmountToSlot(amountToAdd);
                amountLeft -= amountToAdd;
                slotWithItem = FindNextSlotkWithItemNotFull(item);

            }
        }


        ItemSlot slotEmpty = FindNextEmptySlot();
        while (slotEmpty != null && amountLeft > 0)
        {
            int amountToAdd = Mathf.Min(maxStackSize, amountLeft);
            slotEmpty.AddItemToNewSlot(iconInInventory, item, amountToAdd);
            amountLeft -= amountToAdd;
            slotEmpty = FindNextEmptySlot();
        }

        if (amountLeft > 0)
        {
            Debug.Log("Could not add item, inventory is full");
            PlayerManager.Player.SpawnItemFromPlayer(item, amountLeft);
        }

        if (amountLeft < amount)
        {
            InventoryItemInfos itemInfos = FindItemInInventory(item);
            if (itemInfos != null)
            {
                itemInfos.itemAmount += amount - amountLeft;
            }
            else
            {
                itemsInInventory.Add(new InventoryItemInfos(item, amount - amountLeft));
            }
        }

    }

    public void RemoveItemFromInventory(GeneralItem item, int amount) 
    {
        ItemSlot slotWithItem = FindNextSlotkWithItem(item);
        int amountLeft = amount;
        while (slotWithItem != null && amountLeft > 0)
        {
            int currentAmount = slotWithItem.currentItemUI.GetComponent<ItemInInventory>().amountOfItem;
            int amountToRemove = Mathf.Min(currentAmount, amountLeft);
            slotWithItem.RemoveItemAmountFromSlot(amountToRemove);
            amountLeft -= amountToRemove;
            //Debug.Log("Removing " + amountToRemove + itemSO.itemName + " from " + slotWithItem.name + " (Containing "+currentAmount+"). Still "+amountLeft+" to remove.");
            slotWithItem = FindNextSlotkWithItem(item);

        }

        RemoveItemFromInventoryList(item, amount - amountLeft);
        if (amountLeft > 0)
        {
            Debug.Log("Could not remove enough items, no more in inventory");
        }
    }

    public void RemoveItemFromInventory(ItemSlot slotWithItem, int amount)
    {
        int currentAmount = slotWithItem.currentItemUI.amountOfItem;
        if  (currentAmount < amount)
        {
            Debug.Log("Tried to remove too many items from slot, capped to current amount.");
            amount = currentAmount;
        }
        RemoveItemFromInventoryList(slotWithItem.currentItemUI.Item, amount);
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
            if (slot.currentItemUI != null)
            {
                AddItemToInventoryList(slot.currentItemUI.Item, slot.currentItemUI.amountOfItem);
            }
        }
    }

    public void RefreshInventoryItemsinUI()     
    {
        if (itemsInInventory != null)
        {
            foreach (InventoryItemInfos itemInfo in itemsInInventory)
            {
                AddItemToInventory(itemInfo.item, itemInfo.itemAmount);
            }
        }
    }

    public bool IsItemInInventory(GeneralItem item)
    {
        foreach (InventoryItemInfos itemInfo in itemsInInventory)    
        {
            if (item.ItemSO == itemInfo.item.ItemSO) { return true; }
        }
        return false;
    }

    public int AmountOfItemInInventory(GeneralItem item)
    {
        InventoryItemInfos itemInfo = FindItemInInventory(item);
        if (itemInfo == null) return 0;
        else return itemInfo.itemAmount;
    }

    public InventoryItemInfos FindItemInInventory(GeneralItem item)
    {
        foreach (InventoryItemInfos itemInfo in itemsInInventory)
        {
            if (item.ItemSO == itemInfo.item.ItemSO) { return itemInfo; }
        }
        return null;
    }

    private void AddItemToInventoryList(GeneralItem item, int amount)
    {
        if (amount > 0)
        {
            InventoryItemInfos itemInfo = FindItemInInventory(item);
            if (itemInfo != null)
            {
                itemInfo.itemAmount += amount;
            }
            else
            {
                itemsInInventory.Add(new InventoryItemInfos(item, amount));
            }
        }
    }

    private void RemoveItemFromInventoryList(GeneralItem item, int amount)
    {
        InventoryItemInfos itemInfo = FindItemInInventory(item);
        if (itemInfo != null)
        {
            itemInfo.itemAmount -= amount;
            if (itemInfo.itemAmount <= 0)
            {
                itemsInInventory.Remove(itemInfo);
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
            if (slot.currentItemUI == null)
            {
                return slot;
            }
        }
        return null;
    }

    public ItemSlot FindNextSlotkWithItem(GeneralItem item)
    {
        foreach (ItemSlot slot in slotList)
        {
            if (slot.currentItemUI != null)
            {
                if (slot.currentItemUI.ItemSO == item.ItemSO && slot.currentItemUI.amountOfItem > 0)
                {
                    return slot;
                }
            }
        }
        return null;
    }

    public ItemSlot FindNextSlotkWithItemNotFull(GeneralItem item)
    {
        foreach (ItemSlot slot in slotList)
        {
            if (slot.currentItemUI != null)
            {
                if (slot.currentItemUI.ItemSO == item.ItemSO && slot.currentItemUI.amountOfItem < item.ItemSO.maxStackSize)
                {
                    return slot;
                }
            }
        }
        return null;
    }
}
