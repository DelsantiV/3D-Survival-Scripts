using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GoTF.Content
{
    public class ItemPile
    {
        public Sprite PileIcon { get; private set; }

        /// <summary>
        /// All items in the pile
        /// </summary>
        public List<GeneralItem> ItemsInPile { get; private set; }


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

        public Dictionary<Item_General_SO, int> CompactItemsInPile
        {
            get
            {
                Dictionary<Item_General_SO, int> compactItemsInPile = new();
                if (ItemsInPile == null) { return compactItemsInPile; }
                foreach (Item_General_SO itemSO in ItemsSOInPile)
                {
                    if (compactItemsInPile.ContainsKey(itemSO)) { compactItemsInPile[itemSO] += 1; }
                    else compactItemsInPile.Add(itemSO, 1);
                }
                return compactItemsInPile;
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
        /// Wether the pile is empty (contains 0 item) or not
        /// </summary>
        public bool IsEmpty
        {
            get { return ItemsInPile.Count == 0; }
        }

        ///<summary>
        /// The number of different types of items in pile
        /// </summary>
        public int NumberOfDifferentItemsInPile
        {
            get
            {
                return ItemsSOInPile.DistinctBy(itemSo => itemSo.name).Count();
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
        public GeneralItem GetFirstItemInPile
        {
            get
            {
                if (ItemsInPile == null) { return null; }
                return ItemsInPile[0];
            }
        }
        /// <summary>
        /// Returns the item of the pile having this index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GeneralItem GetItemInPile(int index)
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

        /// <summary>
        /// The ItemPileInWorld representing this pile in the world
        /// </summary>
        public ItemPileInWorld ItemPileInWorld { get; private set; }

        public bool IsInWorld
        {
            get { return ItemPileInWorld != null; }
        }

        private ItemPileInInventory itemPileUI;

        public bool IsInInventory
        {
            get { return itemPileUI != null; }
        }

        public UnityEvent OnPileChanged = new UnityEvent();

        /// <summary>
        /// The animation ID when using this pile. For now, only useful when pile is only one item with different animation
        /// </summary>
        public int AnimationID
        {
            get
            {
                if (IsPileUniqueItem) { return GetFirstItemInPile.animationID; }
                else return 0;
            }
        }

        public ItemPile()
        {
            ItemsInPile = new();
        }

        public ItemPile(List<GeneralItem> itemsInPile)
        {
            if (itemsInPile == null) { ItemsInPile = new(); }
            else { ItemsInPile = itemsInPile; }
            if (ItemsInPile.Contains(null))
            {
                Debug.Log("Pile " + ToString() + "contained null items. These were removed.");
                ItemsInPile.RemoveAll(item => item == null);
            }
        }

        /// <summary>
        /// Creates an ItemPile containing this item
        /// </summary>
        /// <param name="item"The Item used to create the pile></param>
        /// <returns> ItemPile </returns>
        public ItemPile(GeneralItem item)
        {
            if (item != null) ItemsInPile = new List<GeneralItem>() { item };
            else ItemsInPile = new();
        }

        /// <summary>
        /// Creates an ItemPile containing these items
        /// </summary>
        /// <param name="itemNamesList"></param>
        public ItemPile(List<string> itemNamesList)
        {
            ItemsInPile = itemNamesList.ConvertAll(itemName => ItemManager.GetItemByName(itemName));
            if (ItemsInPile.Contains(null))
            {
                Debug.Log("Pile " + ToString() + "contained null items. These were removed.");
                ItemsInPile.RemoveAll(item => item == null);
            }
        }

        public ItemPile(string itemName)
        {
            GeneralItem item = ItemManager.GetItemByName(itemName);
            if (item != null) ItemsInPile = new() { item };
        }

        public ItemPile(ItemInWorld worldItem)
        {
            Debug.Log("Creating new Pile from World Item");
            if (worldItem.item != null)
            {
                ItemsInPile = new List<GeneralItem>() { worldItem.item };
                worldItem.RemoveFromPile();
                ItemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
                ItemPileInWorld.InitializeItemPileInWorld(this, worldItem);
                ItemPileInWorld.RemoveRigidBodyFromItems();
            }
            else
            {
                Debug.Log("Failed cause World Item was null item");
            }
        }

        public void DestroyPile() { ItemsInPile.Clear(); }
        public bool IsContainedIn(ItemPile otherPile)
        {
            if (IsEmpty) return true;
            Dictionary<Item_General_SO, int> otherPileCompact = otherPile.CompactItemsInPile;
            foreach(Item_General_SO itemSO in CompactItemsInPile.Keys)
            {
                if (!otherPileCompact.ContainsKey(itemSO)) return false;
                if (CompactItemsInPile[itemSO] > otherPileCompact[itemSO]) return false;
            }
            return true;
        }

        public bool Contains(ItemPile otherPile)
        {
            if (otherPile.IsEmpty) return true;
            Dictionary<Item_General_SO, int> otherPileCompact = otherPile.CompactItemsInPile;
            foreach (Item_General_SO itemSO in otherPileCompact.Keys)
            {
                if (!CompactItemsInPile.ContainsKey(itemSO)) return false;
                if (otherPileCompact[itemSO] > CompactItemsInPile[itemSO]) return false;
            }
            return true;
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
            if (IsInWorld) { ItemPileInWorld.AddItem(item); }
            OnPileChanged?.Invoke();
        }

        /// <summary>
        /// Adds the given world item to the pile
        /// </summary>
        private void AddItemToPile(ItemInWorld worldItem)
        {
            if (!IsInWorld) { return; }
            ItemsInPile.Add(worldItem.item);
            ItemPileInWorld.AddItem(worldItem);
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

        public bool TryAddItemToPile(ItemInWorld worldItem, float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity)
        {
            if (ItemPileInWorld == null) { return false; }
            GeneralItem item = worldItem.item;
            if (item.Weight + this.Weight > maxWeight || item.Bulk + this.Bulk > maxBulk) { return false; } // If pile would be too heavy or too bulky, do not add item
            else
            {
                if (ItemPileInWorld.isRigidBody && worldItem.TryGetComponent(out Rigidbody rb)) { Object.Destroy(rb); }
                AddItemToPile(worldItem);
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
            Debug.Log("Max weight: " + maxWeight.ToString() + "\n Merged piles weight: " + (pileToMerge.Weight + this.Weight).ToString());
            if (pileToMerge.Weight + this.Weight > maxWeight || pileToMerge.Bulk + this.Bulk > maxBulk) { return false; } // If pile would be too heavy or too bulky, do not merge piles
            else
            {
                MergePiles(pileToMerge);
                return true;
            }
        }

        /// <summary>
        /// Merge the given pile with this pile, adding all itelms of the given pile to this pile
        /// </summary>
        /// <param name="pileToMerge" The item pile to merge with></param>
        private void MergePiles(ItemPile pileToMerge)
        {
            if (pileToMerge.NumberOfItemsInPile == 0) { return; }

            if (NumberOfItemsInPile == 0)
            {
                ItemsInPile = pileToMerge.ItemsInPile;
                return;
            }

            List<GeneralItem> newItemsInPile = new List<GeneralItem>(NumberOfItemsInPile + pileToMerge.NumberOfItemsInPile);
            newItemsInPile.AddRange(ItemsInPile);
            newItemsInPile.AddRange(pileToMerge.ItemsInPile);
            ItemsInPile = newItemsInPile;
            if (IsInWorld)
            {
                ItemPileInWorld.MergePile(pileToMerge);
            }
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
            if (NumberOfItemsInPile == 0) { rejectedPile = new(); return null; }

            if (Weight < maxWeight && Bulk < maxBulk) { rejectedPile = new(); return new List<ItemPile>() { this }; }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxWeight"></param>
        /// <param name="maxBulk"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <param name="maxNumberOfDifferentItems"></param>
        /// <returns></returns>
        public ItemPile TakePartOfPile(float maxWeight = Mathf.Infinity, float maxBulk = Mathf.Infinity, int maxNumberOfItems = int.MaxValue, int maxNumberOfDifferentItems = int.MaxValue, bool removeFromOriginalPile = true)
        {
            ItemPile itemPilePart = new ItemPile();

            if (IsEmpty) { return itemPilePart; }

            //If Pile is small enough considering the parameters, return whole pile
            if (Weight < maxWeight && Bulk < maxBulk && NumberOfItemsInPile < maxNumberOfItems && NumberOfDifferentItemsInPile < maxNumberOfDifferentItems)
            {
                return this;
            }

            int numberOfDifferentItems = 0;
            foreach (GeneralItem item in ItemsInPile)
            {
                if (!itemPilePart.IsCorrespondingItemInPile(item.ItemSO)) { numberOfDifferentItems++; }
                if (itemPilePart.NumberOfItemsInPile < maxNumberOfItems && itemPilePart.Weight + item.Weight < maxWeight && itemPilePart.Bulk + item.Bulk < maxBulk && numberOfDifferentItems < maxNumberOfDifferentItems)
                {
                    itemPilePart.AddItemToPile(item);
                }
                else { numberOfDifferentItems--; }

                // Return part of pile if max number of items or max number of different items is reached
                if (itemPilePart.NumberOfItemsInPile == maxNumberOfItems || numberOfDifferentItems == maxNumberOfDifferentItems) { return itemPilePart; }
            }
            if (removeFromOriginalPile && itemPilePart.NumberOfItemsInPile > 0) { foreach (GeneralItem item in itemPilePart.ItemsInPile) { RemoveItemFromPile(item); } }
            return itemPilePart;
        }

        /// <summary>
        /// Returns true if the pile contains this item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(GeneralItem item)
        {
            return ItemsInPile.Contains(item);
        }
        /// <summary>
        /// Returns true if the pile contains an item with this type
        /// </summary>
        /// <param name="itemSO"></param>
        /// <returns></returns>
        public bool IsCorrespondingItemInPile(Item_General_SO itemSO)
        {
            return ItemsSOInPile.Contains(itemSO);
        }
        /// <summary>
        /// Returns all items contained in the pile with this type
        /// </summary>
        /// <param name="itemSO"></param>
        /// <returns></returns>
        public List<GeneralItem> GetAllCorrespondingItems(Item_General_SO itemSO)
        {
            return ItemsInPile.FindAll(item => item.ItemSO == itemSO);
        }
        /// <summary>
        /// Returns the first item contained in the pile from this type
        /// </summary>
        /// <param name="itemSO"></param>
        /// <returns></returns>
        public GeneralItem GetFirstCorrespondingItem(Item_General_SO itemSO)
        {
            return ItemsInPile.Find(item => item.ItemSO == itemSO);
        }

        /// <summary>
        /// Removes this item from the pile
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItemFromPile(GeneralItem item, bool shouldDestroy = false)
        {
            if (Contains(item)) { ItemsInPile.Remove(item); };
            if (IsInWorld) { ItemPileInWorld.RemoveItem(item, shouldDestroy); }
            OnPileChanged?.Invoke();
        }
        public void RemoveItemFromPile(ItemInWorld worldItem, bool shouldDestroy = false)
        {
            if (Contains(worldItem.item)) { ItemsInPile.Remove(worldItem.item); };
            if (IsInWorld) { ItemPileInWorld.RemoveItem(worldItem, shouldDestroy); }
            OnPileChanged?.Invoke();
        }

        public bool TryRemoveItemFromPile(GeneralItem item, bool shouldDestroy = false)
        {
            if (!Contains(item)) return false;
            else
            {
                RemoveItemFromPile(item, shouldDestroy);
                return true;
            }
        }

        public bool TryRemoveItemFromPile(ItemInWorld worldItem, bool shouldDestroy = false)
        {
            if (!Contains(worldItem.item)) return false;
            else
            {
                RemoveItemFromPile(worldItem, shouldDestroy);
                return true;
            }
        }

        public void RemoveFirstCorrespondingItem(Item_General_SO itemSO, bool shouldDestroy = false)
        {
            RemoveItemFromPile(GetFirstCorrespondingItem(itemSO), shouldDestroy);
        }

        public void RemoveAllCorrespondingItems(Item_General_SO itemSO, bool shouldDestroy = false)
        {
            foreach (GeneralItem item in GetAllCorrespondingItems(itemSO)) { RemoveItemFromPile(item, shouldDestroy); }
        }

        public bool TryRemovePile(ItemPile pile, bool shouldDestroy = false)
        {
            if (!Contains(pile)) return false;
            else
            {
                foreach (Item_General_SO itemSO in pile.ItemsSOInPile) RemoveFirstCorrespondingItem(itemSO, shouldDestroy);
                return true;
            }
        }

        /// <summary>
        /// Spawns the pile at the given position in world
        /// </summary>
        /// <param name="spawnPosition"></param>
        /// <returns></returns>
        public ItemPileInWorld SpawnInWorld(Vector3 spawnPosition)
        {
            Debug.Log("Spawning item pile : " + ToString());
            ItemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
            ItemPileInWorld.SpawnItemPile(this, spawnPosition);
            return ItemPileInWorld;
        }
        /// <summary>
        /// Spawns the pile as a child of the given target Transform
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <returns></returns>
        public ItemPileInWorld SpawnInWorld(Transform targetTransform)
        {
            Debug.Log("Spawning item pile : " + ToString());
            ItemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
            ItemPileInWorld.SpawnItemPile(this, targetTransform);
            return ItemPileInWorld;
        }
        /// <summary>
        /// Spawns the pile as a child of the given target Transform and at given LOCAL position and rotation (relative to the target Transform) 
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="spawnRotation"></param>
        /// <returns></returns>
        public ItemPileInWorld SpawnInWorld(Transform targetTransform, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            Debug.Log("Spawning item pile : " + ToString());
            ItemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
            ItemPileInWorld.SpawnItemPile(this, targetTransform, spawnPosition, spawnRotation);
            return ItemPileInWorld;
        }
        /// <summary>
        /// Spawns the pile as a child of the given hand Transform, and without adding Rigidbodies to the items of the pile 
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public ItemPileInWorld SpawnInHands(Transform hand)
        {
            Debug.Log("Spawning item pile : " + ToString());
            ItemPileInWorld = new GameObject("Pile " + ToString()).AddComponent<ItemPileInWorld>();
            ItemPileInWorld.SpawnItemPile(this, hand, false, true);
            return ItemPileInWorld;
        }

        public void DestroyItemPileInWorld()
        {
            if (IsInWorld) { ItemPileInWorld.DestroyItemPile(); }
        }
        /// <summary>
        /// If the pile is only one item, uses the item
        /// </summary>
        /// <param name="player"></param>
        /// <param name="item"></param>
        public void Use(PlayerManager player, EquippedItemPile item)
        {
            if (IsPileUniqueItem) { GetFirstItemInPile.UseItem(player, item); }
        }
        public void StopUse(PlayerManager player, EquippedItemPile item)
        {
            if (IsPileUniqueItem) { GetFirstItemInPile.StopUsingItem(player, item); }
        }
        /// <summary>
        /// What happens when a collision is detected
        /// </summary>
        /// <param name="other"></param>
        public void OnCollisionDetected(Collider other)
        {
            if (!IsInWorld) return;
            if (!IsInInventory) return;
            if (IsPileUniqueItem) { GetFirstItemInPile.OnCollisionDetected(other); }
        }

        public override string ToString()
        {
            if (NumberOfItemsInPile == 0) { return "Empty Pile"; }
            string pileName = "";
            foreach (Item_General_SO itemSO in ItemsSOInPile)
            {
                if (itemSO != null)
                {
                    pileName = pileName + ", " + itemSO.name;
                }
            }
            if (pileName != "") { pileName = pileName[2..]; }
            return pileName;
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
        /// <summary>
        /// Merges two piles in one retaining all items of the two merged piles. Given piles are not destroyed !
        /// </summary>
        /// <param name="itemPile1"></param>
        /// <param name="itemPile2"></param>
        /// <returns></returns>
        public static ItemPile MergePiles(ItemPile itemPile1, ItemPile itemPile2)
        {
            List<GeneralItem> newItemsInPile = new List<GeneralItem>(itemPile1.NumberOfItemsInPile + itemPile2.NumberOfItemsInPile);
            newItemsInPile.AddRange(itemPile1.ItemsInPile);
            newItemsInPile.AddRange(itemPile2.ItemsInPile);
            return new ItemPile(newItemsInPile);
        }

        /// <summary>
        /// Tries to merge two piles given a maximum weight and a maximum bulk. Returns true if was able to merged
        /// </summary>
        /// <param name="itemPile1"></param>
        /// <param name="itemPile2"></param>
        /// <param name="resultPile" The pile resulting from the merging operation"," is null when merging failed></param>
        /// <param name="maxWeight"></param>
        /// <param name="maxBulk"></param>
        /// <returns></returns>
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
                foreach (GeneralItem item in extractedPile.ItemsInPile)
                {
                    orderedItems.Remove(item);
                }
            }
            if (orderedItems.Count > 0) { suppItems = new(orderedItems); }
            else { suppItems = new(); }
            return splittedItemPiles;
        }

        public static bool ArePileCorresponding(ItemPile pile1, ItemPile pile2)
        {
            Dictionary<Item_General_SO, int> pile1Compact = pile1.CompactItemsInPile;
            Dictionary<Item_General_SO, int> pile2Compact = pile2.CompactItemsInPile;
            if (pile1Compact.Count != pile2Compact.Count) return false;
            foreach (Item_General_SO itemSO in pile1Compact.Keys)
            {
                if (!pile2Compact.ContainsKey(itemSO)) return false;
                if (pile1Compact[itemSO] != pile2Compact[itemSO]) return false;
            }
            return true;
        }
    }
}
