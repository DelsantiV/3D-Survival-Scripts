using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPile
{

    /// <summary>
    /// All items in the pile
    /// </summary>
    public List<GeneralItem> ItemsInPile {get; private set;}


    /// <summary>
    /// Items SO of all items in the pile
    /// </summary>
    public List<Item_General_SO> ItemsSOInPile
    {
        get 
        {
            return ItemsInPile.ConvertAll(item => item.ItemSO);
        }
    }

    /// <summary>
    /// True if pile contains only one item, false otherwise (even if pile is null)
    /// </summary>
    public bool IsPileUniqueItem
    {
        get
        {
            if (ItemsInPile == null) { return false; }
            return ItemsInPile.Count == 1;
        }
    }

    /// <summary>
    /// Returns the first item of the pile
    /// </summary>
    public GeneralItem FirstItemInPile
    {
        get
        {
            return ItemsInPile[0];
        }
    }

    public GeneralItem ItemInPile(int index)
    {
        return ItemsInPile[index];
    }


    /// <summary>
    /// Total weight of the pile (sum of items weights)
    /// </summary>
    public float Weight
    {
        get
        {
            return ItemsInPile.Sum(item => item.Weight);
        }
    }

    /// <summary>
    /// Total bulk of the pile
    /// </summary>
    // For now total bulk is sum of items bulks. Future : increase bulk when carrying different kind of items
    public float Bulk
    {
        get
        {
            return ItemsInPile.Sum(item => item.Bulk);
        }
    }

    public ItemPile()
    {

    }

    public ItemPile(List<GeneralItem> itemsInPile)
    {
        this.ItemsInPile = itemsInPile;
    }


    /// <summary>
    /// Creates an ItemPile containing this item
    /// </summary>
    /// <param name="item"></param>
    /// <returns> ItemPile </returns>
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
        Debug.Log("Spawning item pile : " + ToString());
        ItemPileInWorld itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, spawnPosition);
        return itemPileInWorld;
    }
    public ItemPileInWorld SpawnInWorld(Transform targetTransform)
    {
        Debug.Log("Spawning item pile : " + ToString());
        ItemPileInWorld itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, targetTransform);
        return itemPileInWorld;
    }

    public ItemPileInWorld SpawnInHands(Transform hand)
    {
        Debug.Log("Spawning item pile : " + ToString());
        ItemPileInWorld itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, hand, false);
        return itemPileInWorld;
    }


    public void Action(PlayerManager player)
    {

    }

    public override string ToString()
    {
        string pileName = "";
        foreach (Item_General_SO itemSO in ItemsSOInPile) { pileName = pileName + ", " + itemSO.name; }
        return pileName[2..];
    }
}
