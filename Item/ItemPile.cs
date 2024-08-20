using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPile
{
    public List<GeneralItem> ItemsInPile {get; private set;}

    public List<Item_General_SO> ItemsSOInPile
    {
        get 
        {
            return ItemsInPile.ConvertAll(item => item.ItemSO);
        }
    }

    public bool isPileUniqueItem
    {
        get
        {
            if (ItemsInPile == null) { return false; }
            return ItemsInPile.Count == 1;
        }
    }

    public GeneralItem FirstItemInPile
    {
        get
        {
            return ItemsInPile[0];
        }
    }

    public ItemPile()
    {

    }

    public ItemPile(List<GeneralItem> itemsInPile)
    {
        this.ItemsInPile = itemsInPile;
    }

    public ItemPile(GeneralItem item)
    {
        ItemsInPile = new List<GeneralItem>() { item };
    }

    public virtual ItemInInventory CreateItemInInventory()
    {
        ItemInInventory itemUI = ItemManager.ItemUITemplate;
        itemUI.SetItem(this);
        return itemUI;
    }
    public virtual void SetItemToSlot(ItemSlot slot)
    {
        slot.SetItemToSlot(CreateItemInInventory());
    }

    public void AddItemToPile(GeneralItem item)
    {
        ItemsInPile.Add(item);
    }

    public bool IsItemInPile(GeneralItem item)
    {
        return ItemsInPile.Contains(item);
    }
    public bool IsCorrespondingItemInPile(Item_General_SO itemSO)
    {
        return ItemsSOInPile.Contains(itemSO);
    }

    public List<GeneralItem> GetAllCorrespondingItems(Item_General_SO itemSO)
    {
        return ItemsInPile.FindAll(item => item.ItemSO == itemSO);
    }
    public GeneralItem GetFirstCorrespondingItem(Item_General_SO itemSO)
    {
        return ItemsInPile.Find(item => item.ItemSO == itemSO);
    }

    public void RemoveItemFromPile(GeneralItem item)
    {
        if (IsItemInPile(item)) { ItemsInPile.Remove(item); };
    }

    public void RemoveFirstCorrespondingItem(Item_General_SO itemSO)
    {
        RemoveItemFromPile(GetFirstCorrespondingItem(itemSO));
    }

    public void RemoveAllCorrespondingItems(Item_General_SO itemSO)
    {
        foreach (GeneralItem item in GetAllCorrespondingItems(itemSO)) { RemoveItemFromPile(item); }
    }

    public void SpawnInWorld(Vector3 spawnPosition)
    {
        ItemPileInWorld itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, spawnPosition);
    }

    public void Action(PlayerManager player)
    {

    }

    public override string ToString()
    {
        string pileName = "";
        foreach (Item_General_SO itemSO in ItemsSOInPile) { pileName = pileName + ", " + itemSO.name; }
        return pileName;
    }
}
