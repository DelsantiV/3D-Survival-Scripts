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
            if (ItemsInPile == null) { return new(); }
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
            if (ItemsInPile == null) { return null; }
            return ItemsInPile[0];
        }
    }

    public GeneralItem ItemInPile(int index)
    {
        if (ItemsInPile == null) { return null; }
        if (index < 0 || index > ItemsInPile.Count) { return null; }
        return ItemsInPile[index];
    }


    /// <summary>
    /// Total weight of the pile (sum of items weights)
    /// </summary>
    public float Weight
    {
        get
        {
            if (ItemsInPile == null) { return 0f; }
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
            if (ItemsInPile == null) { return 0f; }
            return ItemsInPile.Sum(item => item.Bulk);
        }
    }

    private ItemPileInWorld itemPileInWorld;

    public bool IsInWorld
    {
        get { return itemPileInWorld != null; }
    }

    private ItemPileInInventory itemPileUI;

    public bool IsInInventory
    {
        get { return itemPileUI != null; }
    }

    public UnityEvent OnPileChanged = new UnityEvent();

    public ItemPile()
    {
        ItemsInPile = new();
    }

    public ItemPile(List<GeneralItem> itemsInPile)
    {
        if (itemsInPile == null) { ItemsInPile = new(); }
        else { ItemsInPile = itemsInPile; }
    }

    /// <summary>
    /// Creates an ItemPile containing this item
    /// </summary>
    /// <param name="item"The Item used to create the pile></param>
    /// <returns> ItemPile </returns>
    public ItemPile(GeneralItem item)
    {
        ItemsInPile = new List<GeneralItem>() { item };
    }

    /// <summary>
    /// Creates an ItemPile containing these items
    /// </summary>
    /// <param name="itemNamesList"></param>
    public ItemPile(List<string> itemNamesList)
    {
        ItemsInPile = itemNamesList.ConvertAll(itemName => ItemManager.GetItemByName(itemName)); ;
    }


    /// <summary>
    /// Set the pile to an inventory slot, creating an <c>ItemPileInInventory</c> if necessary
    /// </summary>
    /// <param name="slot" The slot the pile should be set to></param>
    public virtual void SetItemPileToSlot(ItemSlot slot)
    {
        itemPileUI = Object.Instantiate(ItemManager.PileIconTemplate, slot.transform);
        slot.SetItemPileToSlot(itemPileUI);
        itemPileUI.SetItemPile(this, slot.Player);
    }

    /// <summary>
    /// Adds the given item to the pile
    /// </summary>
    private void AddItemToPile(GeneralItem item)
    {
        ItemsInPile.Add(item);
        if (IsInWorld) { itemPileInWorld.AddItem(item); }
        OnPileChanged?.Invoke();
    }


    /// <summary>
    /// Try to add the given item to the pile
    /// </summary>
    /// <param name="item" The item to add to the pile></param>
    /// <param name="maxWeight" The maximum weight the pile should reach (default is infinity)></param>
    /// <param name="maxBulk" The maximum bulk the pile should reach (default is infinity)></param>
    /// <returns>True if the item was added, false otherwise</returns>
    public bool TryAddItemToPile(GeneralItem item, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity)
    {
        if (item.Weight + this.Weight > maxWeight || item.Bulk + this.Bulk > maxBulk) { return false; } // If pile would be too heavy or too bulky, do not add item
        else
        {
            AddItemToPile(item);
            return true;
        }
    }


    /// <summary>
    /// Try to merge the given pile with this pile
    /// </summary>
    /// <param name="pileToMerge" The item pile to merge with the pile></param>
    /// <param name="maxWeight" The maximum weight the resulting pile should reach (default is infinity)></param>
    /// <param name="maxBulk" The maximum bulk the resulting pile should reach (default is infinity)></param>
    /// <returns>True if the pile was merged, false otherwise</returns>
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
        if (pileToMerge.NumberOfItemsInPile == 0) { return; }

        if (NumberOfItemsInPile == 0) 
        { 
            ItemsInPile = pileToMerge.ItemsInPile;
            return;
        }

        List<GeneralItem> newItemsInPile = new List<GeneralItem> (NumberOfItemsInPile + pileToMerge.NumberOfItemsInPile);
        newItemsInPile.AddRange(ItemsInPile);
        newItemsInPile.AddRange(pileToMerge.ItemsInPile);
        ItemsInPile = newItemsInPile;
        OnPileChanged?.Invoke();
    }



    /// <summary>
    /// Splitting the pile into multiple piles (note that this pile is not destroyed, so all items will have two instances)
    /// </summary>
    /// <param name="rejectedPile" The pile formed by all items exceeding the control parameters></param>
    /// <param name="maxNumberOfPiles" The maximum number of piles to form while splitting excluding <c>rejectedPile<\c><\param>
    /// <param name="maxWeight" The maximum weight an extracted pile should reach (default is infinity)></param>
    /// <param name="maxBulk" The maximum bulk an extracted pile should reach (default is infinity)></param>
    /// <param name="splitMethod" The paramter to use to order items before splitting: weight, bulk or both></param> 
    /// <returns>List of item piles with count smaller than <c>maxNumberOfPiles</c></returns>
    public List<ItemPile> SplitItemPile(out ItemPile rejectedPile, int maxNumberOfPiles = 0, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity, ItemPilesUtilities.SplitMethod splitMethod = ItemPilesUtilities.SplitMethod.Weight)
    {
        if (NumberOfItemsInPile == 0) {  rejectedPile = new(); return null; }

        if (Weight < maxWeight && Bulk < maxBulk) { rejectedPile = new(); return new List<ItemPile>() {this}; }

        List<GeneralItem> tooHeavyItems = ItemsInPile.FindAll(item => item.Weight > maxWeight);
        if (tooHeavyItems.Count > 0) { ItemsInPile.RemoveAll(item => item.Weight > maxWeight); }
        List<GeneralItem> tooBulkyItems = ItemsInPile.FindAll(item => item.Bulk > maxBulk);
        if (tooBulkyItems.Count > 0) { ItemsInPile.RemoveAll(item => item.Bulk > maxBulk); }
        rejectedPile = new(tooHeavyItems.Concat(tooBulkyItems).ToList());

        if (splitMethod == ItemPilesUtilities.SplitMethod.Both)
        {

            List<GeneralItem> weightOrderedItems = ItemsInPile.OrderByDescending(item => item.Weight).ToList();
            List<GeneralItem> bulkOrderedItems = ItemsInPile.OrderByDescending(item => item.Bulk).ToList();

            List<ItemPile> weightSplittedPile = ItemPilesUtilities.SplitItemPileFromOrderedItemList(weightOrderedItems, out ItemPile suppItemPileWeight, maxNumberOfPiles, maxWeight, maxBulk);
            List<ItemPile> bulkSplittedPile = ItemPilesUtilities.SplitItemPileFromOrderedItemList(bulkOrderedItems, out ItemPile suppItemPileBulk, maxNumberOfPiles, maxWeight, maxBulk);

            if (weightOrderedItems.Count < bulkSplittedPile.Count) 
            {
                rejectedPile.MergePiles(suppItemPileWeight);
                return weightSplittedPile; 
            }
            else 
            {
                rejectedPile.MergePiles(suppItemPileWeight);
                return bulkSplittedPile; 
            }
        }
        
        else
        {
            List<GeneralItem> orderedItems = new();
            if (splitMethod == ItemPilesUtilities.SplitMethod.None) { orderedItems = ItemsInPile; }
            else if (splitMethod == ItemPilesUtilities.SplitMethod.Weight) { orderedItems = ItemsInPile.OrderByDescending(item => item.Weight).ToList(); }
            else if (splitMethod == ItemPilesUtilities.SplitMethod.Bulk) { orderedItems = ItemsInPile.OrderByDescending(item => item.Bulk).ToList(); }

            List<ItemPile> splittedPile = ItemPilesUtilities.SplitItemPileFromOrderedItemList(orderedItems, out ItemPile suppItemPile, maxNumberOfPiles, maxWeight, maxBulk);
            rejectedPile.MergePiles(suppItemPile);
            return splittedPile;
        }

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
        itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, spawnPosition);
        return itemPileInWorld;
    }
    public ItemPileInWorld SpawnInWorld(Transform targetTransform)
    {
        Debug.Log("Spawning item pile : " + ToString());
        itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, targetTransform);
        return itemPileInWorld;
    }

    public ItemPileInWorld SpawnInHands(Transform hand)
    {
        Debug.Log("Spawning item pile : " + ToString());
        itemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
        itemPileInWorld.SpawnItemPile(this, hand, false);
        return itemPileInWorld;
    }

    public void DestroyItemPileInWorld()
    {
        if (IsInWorld) { itemPileInWorld.DestroyItemPile(); }
    }


    public void Action(PlayerManager player)
    {

    }

    public override string ToString()
    {
        if (NumberOfItemsInPile == 0) { return "Empty Pile"; }
        string pileName = "";
        foreach (Item_General_SO itemSO in ItemsSOInPile) { pileName = pileName + ", " + itemSO.name; }
        return pileName[2..];
    }
}

public static class ItemPilesUtilities
{
    public enum SplitMethod
    {
        Weight,
        Bulk,
        Both,
        None
    }
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
        if (itemPile1 == null && itemPile2 == null) 
        { 
            resultPile = null;
            return false;
        }

        if (itemPile1 == null)
        {
            resultPile = itemPile2;
            return true;
        }

        if (itemPile2 == null)
        {
            resultPile = itemPile1;
            return true;
        }

        if (itemPile1.Weight + itemPile2.Weight > maxWeight || itemPile1.Bulk + itemPile2.Bulk > maxBulk) { return false; } // If pile would be too heavy or too bulky, do not merge piles
        else
        {
            resultPile = MergePiles(itemPile1, itemPile2);
            return true;
        }
    }

    public static List<ItemPile> SplitItemPileFromOrderedItemList(List<GeneralItem> orderedItems, out ItemPile suppItems, int maxNumberOfPiles = 0, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity)
    {
        List<ItemPile> splittedItemPiles = new();
        if (maxNumberOfPiles == 0) { maxNumberOfPiles = orderedItems.Count; }
        while (orderedItems.Count > 0 && splittedItemPiles.Count < maxNumberOfPiles)
        {
            ItemPile extractedPile = new();
            foreach (GeneralItem item in orderedItems)
            {
                extractedPile.TryAddItemToPile(item, maxWeight, maxBulk);
            }
            splittedItemPiles.Add(extractedPile);
            foreach(GeneralItem item in extractedPile.ItemsInPile)
            {
                orderedItems.Remove(item);
            }
        }
        if (orderedItems.Count > 0) { suppItems = new(orderedItems); }
        else {  suppItems = new(); }
        return splittedItemPiles;
    }
}
