using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemPile
{
    public Sprite PileIcon { get; private set; }

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
            return ItemsInPile.ConvertAll(item => item?.ItemSO);
        }
    }

    /// <summary>
    /// The number of items in the pile 
    /// </summary>
    public int NumberOfItemsInPile
    {
        get
        {
            if (ItemsInPile == null) return 0;
            else return ItemsInPile.Count;
        }
    }

    /// <summary>
    /// True if pile contains only one item, false otherwise (even if pile is null)
    /// </summary>
    public bool IsPileUniqueItem
    {
        get
        {
            return NumberOfItemsInPile == 1;
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

    public UnityEvent OnPileChanged = new UnityEvent();

    public ItemPile()
    {

    }

    public ItemPile(List<GeneralItem> itemsInPile)
    {
        ItemsInPile = itemsInPile;
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

    /// <summary>
    /// Creates an ItemPile containing these items
    /// </summary>
    /// <param name="itemNamesList"></param>
    /// <returns> ItemPile </returns>
    public ItemPile(List<string> itemNamesList)
    {
        ItemsInPile = itemNamesList.ConvertAll(itemName => ItemManager.GetItemByName(itemName)); ;
    }

    public virtual void SetItemPileToSlot(ItemSlot slot)
    {
        ItemPileInInventory pileUI = Object.Instantiate(ItemManager.PileIconTemplate, slot.transform);
        slot.SetItemPileToSlot(pileUI);
        pileUI.SetItemPile(this, slot.Player);
    }

    private void AddItemToPile(GeneralItem item)
    {
        ItemsInPile.Add(item);
        OnPileChanged?.Invoke();
    }

    public bool TryAddItemToPile(GeneralItem item, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity)
    {
        if (item.Weight + this.Weight > maxWeight || item.Bulk + this.Bulk > maxBulk) { return false; } // If pile would be too heavy or too bulky, do not add item
        else
        {
            AddItemToPile(item);
            return true;
        }
    }

    public bool TryMergePile(ItemPile pileToMerge, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity) 
    {
        if (pileToMerge.Weight + this.Weight > maxWeight || pileToMerge.Bulk + this.Bulk > maxBulk) { return false; } // If pile would be too heavy or too bulky, do not merge piles
        else
        {
            MergePiles(pileToMerge);
            return true;
        }
    }

    private void MergePiles(ItemPile pileToMerge)
    {
        List<GeneralItem> newItemsInPile = new List<GeneralItem> (NumberOfItemsInPile + pileToMerge.NumberOfItemsInPile);
        newItemsInPile.AddRange(ItemsInPile);
        newItemsInPile.AddRange(pileToMerge.ItemsInPile);
        ItemsInPile = newItemsInPile;
        OnPileChanged?.Invoke();
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
        OnPileChanged?.Invoke();
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

public static class ItemPilesUtilities
{
    public static ItemPile MergePiles(ItemPile itemPile1, ItemPile itemPile2)
    {
        List<GeneralItem> newItemsInPile = new List<GeneralItem>(itemPile1.NumberOfItemsInPile + itemPile2.NumberOfItemsInPile);
        newItemsInPile.AddRange(itemPile1.ItemsInPile);
        newItemsInPile.AddRange(itemPile2.ItemsInPile);
        return new ItemPile(newItemsInPile);
    }


    public static bool TryMergePiles(ItemPile itemPile1, ItemPile itemPile2, out ItemPile resultPile, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity)
    {
        resultPile = null;
        if (itemPile1.Weight + itemPile2.Weight > maxWeight || itemPile1.Bulk + itemPile2.Bulk > maxBulk) { return false; } // If pile would be too heavy or too bulky, do not merge piles
        else
        {
            resultPile = MergePiles(itemPile1, itemPile2);
            return true;
        }
    }
}
