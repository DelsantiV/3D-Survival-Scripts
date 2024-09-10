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

    public bool IsPileUniqueItem
    {
        get
        {
            if (ItemsInPile == null) { return false; }
            return ItemsInPile.Count == 1;
        }
    }

    public bool IsPileUseable
    {
        get
        {
            return false;
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

    public virtual void SetItemPileToSlot(ItemSlot slot)
    {
        ItemPileInInventory pileUI = Object.Instantiate(ItemManager.ItemUITemplate, slot.transform);
        slot.SetItemPileToSlot(pileUI);
        pileUI.SetItemPile(this, slot.Player);
    }

    private void AddItemToPile(GeneralItem item)
    {
        ItemsInPile.Add(item);
    }

    public bool TryAddItemToPile(GeneralItem item)
    {
        AddItemToPile(item);
        return true;
    }

    public bool TryAddPile(ItemPile pile) 
    { 
        return false;
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

    public ItemPileInWorld SpawnInWorld(Vector3 spawnPosition)
    {
        ItemPileInWorld itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, spawnPosition);
        return itemPileInWorld;
    }
    public ItemPileInWorld SpawnInWorld(Transform targetTransform)
    {
        ItemPileInWorld itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, targetTransform);
        return itemPileInWorld;
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
