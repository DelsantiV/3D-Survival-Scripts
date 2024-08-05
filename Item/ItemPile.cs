using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPile : ICarryable
{
    private List<GeneralItem> itemsInPile;

    private List<Item_General_SO> itemsSOInPile
    {
        get 
        {
            return itemsInPile.ConvertAll(item => item.ItemSO);
        }
    }


    public ItemPile()
    {

    }

    public ItemPile(List<GeneralItem> itemsInPile)
    {
        this.itemsInPile = itemsInPile;
    }

    public void AddItemToPile(GeneralItem item)
    {
        itemsInPile.Add(item);
    }

    public bool IsItemInPile(GeneralItem item)
    {
        return itemsInPile.Contains(item);
    }
    public bool IsCorrespondingItemInPile(Item_General_SO itemSO)
    {
        return itemsSOInPile.Contains(itemSO);
    }

    public List<GeneralItem> GetAllCorrespondingItems(Item_General_SO itemSO)
    {
        return itemsInPile.FindAll(item => item.ItemSO == itemSO);
    }
    public GeneralItem GetFirstCorrespondingItem(Item_General_SO itemSO)
    {
        return itemsInPile.Find(item => item.ItemSO == itemSO);
    }

    public void RemoveItemFromPile(GeneralItem item)
    {
        if (IsItemInPile(item)) { itemsInPile.Remove(item); };
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
        
    }

    public void Action(PlayerManager player)
    {

    }
}
