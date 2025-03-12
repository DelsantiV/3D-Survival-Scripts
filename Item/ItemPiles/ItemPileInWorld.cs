using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoTF.Content
{
    public class ItemPileInWorld : MonoBehaviour
    {
        public ItemPile ItemPile { get; private set; }
        private List<ItemInWorld> worldItems;
        public List<GeneralItem> ItemsInPile { get { return ItemPile.ItemsInPile; } }
        private float pileHeight;
        public bool isRigidBody;
        public float Weight
        {
            get { return ItemPile.Weight; }
        }

        public float Bulk
        {
            get { return ItemPile.Bulk; }
        }

        public void InitializeItemPileInWorld(ItemPile itemPile, ItemInWorld worldItem)
        {
            this.ItemPile = itemPile;
            worldItems = new();
            //transform.position = worldItem.transform.position;
            transform.position = Vector3.zero;
            worldItem.transform.SetParent(transform, false);
            worldItems.Add(worldItem);
        }

        /// <summary>
        /// Spawns the item pile at the given position.
        /// </summary>
        /// <param name="itemPile" The ItemPile to spawn></param>
        /// <param name="spawnPosition" The spawning position in world></param>
        /// <param name="isRigidBody" Should the items have a RigidBody></param>
        public void SpawnItemPile(ItemPile itemPile, Vector3 spawnPosition, bool isRigidBody = true)
        {
            this.ItemPile = itemPile;
            worldItems = new();
            this.isRigidBody = isRigidBody;
            transform.position = spawnPosition;
            if (itemPile != null)
            {
                pileHeight = 0;
                foreach (GeneralItem item in ItemPile.ItemsInPile)
                {
                    SpawnIndividualItem(item, isRigidBody, pileHeight);
                    pileHeight += 1;
                }
            }
        }

        /// <summary>
        /// Spawns the item pile as a child of the target Transform, with given LOCAL position and rotation (relative to the target transform)
        /// </summary>
        /// <param name="itemPile" The ItemPile to spawn></param>
        /// <param name="targetTransform" The target Transform></param>
        /// <param name="spawnPosition" The local position relative to the target Transform></param>
        /// <param name="spawnRotation" The local rotation relative to the target Transform></param>
        /// <param name="isRigidBody" Should the items have a RigidBody></param>
        public void SpawnItemPile(ItemPile itemPile, Transform targetTransform, Vector3 spawnPosition, Quaternion spawnRotation, bool isRigidBody = true, bool spawnInHands = false)
        {
            this.ItemPile = itemPile;
            worldItems = new();
            this.isRigidBody = isRigidBody;
            transform.SetParent(targetTransform);
            transform.SetLocalPositionAndRotation(spawnPosition, spawnRotation);
            if (itemPile != null)
            {
                if (spawnInHands && itemPile.IsPileUniqueItem) { SpawnIndividualItemInHand(itemPile.GetFirstItemInPile); }
                else
                {
                    foreach (GeneralItem item in ItemPile.ItemsInPile)
                    {
                        SpawnIndividualItem(item, isRigidBody, out float previousItemHeight, pileHeight);
                        pileHeight += previousItemHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Spawns the item pile as a child of the target Transform
        /// </summary>
        /// <param name="itemPile" The ItemPile to spawn></param>
        /// <param name="targetTransform" The target Transform></param>
        /// <param name="isRigidBody" Should the items have a RigidBody></param>
        public void SpawnItemPile(ItemPile itemPile, Transform targetTransform, bool isRigidBody = true, bool spawnInHands = false)
        {
            SpawnItemPile(itemPile, targetTransform, Vector3.zero, Quaternion.identity, isRigidBody, spawnInHands);
        }

        private void SpawnIndividualItem(GeneralItem item, bool isRigidBody, out float prefabHeight, float height = 0)
        {
            prefabHeight = 0;
            if (item != null)
            {
                if (item.ItemPrefab != null)
                {
                    GameObject itemPrefab = Instantiate(item.ItemPrefab, transform, false);
                    BoxCollider prefabCollider = itemPrefab.GetComponent<BoxCollider>();
                    prefabHeight = prefabCollider.size.y * itemPrefab.transform.localScale.y;
                    itemPrefab.transform.SetLocalPositionAndRotation(item.InPilePosition + Vector3.up * height, item.InPileRotation);
                    ItemInWorld itemInWorld = itemPrefab.AddComponent<ItemInWorld>();
                    itemInWorld.item = item;
                    if (isRigidBody) { itemPrefab.AddComponent<Rigidbody>(); }
                    else { prefabCollider.isTrigger = true; }
                    worldItems.Add(itemInWorld);
                }
            }
        }
        private void SpawnIndividualItem(GeneralItem item, bool isRigidBody, float height = 0)
        {
            if (item != null)
            {
                if (item.ItemPrefab != null)
                {
                    GameObject itemPrefab = Instantiate(item.ItemPrefab, transform);
                    BoxCollider prefabCollider = itemPrefab.GetComponent<BoxCollider>();
                    itemPrefab.transform.SetLocalPositionAndRotation(item.InPilePosition + Vector3.up * height, item.InPileRotation);
                    ItemInWorld itemInWorld = itemPrefab.AddComponent<ItemInWorld>();
                    itemInWorld.item = item;
                    if (isRigidBody) { itemPrefab.AddComponent<Rigidbody>(); }
                    else { prefabCollider.isTrigger = true; }
                    worldItems.Add(itemInWorld);
                }
            }
        }

        private void SpawnIndividualItemInHand(GeneralItem item)
        {
            if (item != null)
            {
                if (item.ItemPrefab != null)
                {
                    GameObject itemPrefab = Instantiate(item.ItemPrefab, transform);
                    BoxCollider prefabCollider = itemPrefab.GetComponent<BoxCollider>();
                    itemPrefab.transform.SetLocalPositionAndRotation(item.InHandPosition, item.InHandRotation);
                    ItemInWorld itemInWorld = itemPrefab.AddComponent<ItemInWorld>();
                    itemInWorld.item = item;
                    prefabCollider.isTrigger = true; 
                    worldItems.Add(itemInWorld);
                }
            }
        }

        private void ParentItemToPile(ItemInWorld worldItem, bool isRigidBody, out float prefabHeight, float height = 0)
        {
            prefabHeight = 0;
            if (worldItem != null)
            {
                worldItem.transform.SetParent(transform, false);
                if (!worldItem.TryGetComponent<BoxCollider>(out BoxCollider prefabCollider)) { prefabCollider = worldItem.GetComponentInChildren<BoxCollider>(); }
                if (prefabCollider != null) prefabHeight = prefabCollider.size.y * worldItem.transform.localScale.y;
                worldItem.transform.localPosition = Vector3.up * height;
                if (isRigidBody && (worldItem.gameObject.GetComponent<Rigidbody>() == null)) { worldItem.gameObject.AddComponent<Rigidbody>(); }
                else { prefabCollider.isTrigger = true; }
                worldItems.Add(worldItem);
            }
        }

        public void AddItem(GeneralItem item, bool applyHeight = true)
        {
            SpawnIndividualItem(item, isRigidBody, out pileHeight, applyHeight ? pileHeight : 0);
        }

        public void AddItem(ItemInWorld item, bool applyHeight = true)
        {
            ParentItemToPile(item, isRigidBody, out pileHeight, applyHeight ? pileHeight : 0);
        }

        public void MergePile(ItemPile pileToMerge, bool applyHeight = true)
        {
            foreach (GeneralItem item in pileToMerge.ItemsInPile)
            {
                SpawnIndividualItem(item, isRigidBody, out float previousItemHeight, applyHeight ? pileHeight : 0);
                if (applyHeight) { pileHeight += previousItemHeight; }
            }
        }
        /// <summary>
        /// Remove an item prefab from the visible pile
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(ItemInWorld worldItem, bool shouldDestroy = false)
        {
            if (!worldItems.Contains(worldItem))
            {
                Debug.Log("Removing " + worldItem.ItemName + " from pile " + ItemPile.ToString() + " failed: no such item found");
                return;
            }
            worldItems.Remove(worldItem);
            if (ItemPile.Contains(worldItem.item)) ItemPile.RemoveItemFromPile(worldItem.item);
            // Should recalculate pile Height

            // Suppose that item is already removed from itemPile

            // If Pile is now empty, destroy whole GameObject

            worldItem.transform.parent = null;
            Debug.Log("Item " + worldItem.ItemName + " removed");
            if (shouldDestroy) 
            {
                Debug.Log("Destroying");
                Destroy(worldItem.gameObject);
            }

            if (worldItems.Count == 0)
            {
                Destroy(gameObject);
            }
        }

        public void RemoveItem(int index)
        {
            if (index < 0 || index >= worldItems.Count)
            {
                Debug.Log("Removing item number " + index + " from pile " + ItemPile.ToString() + " failed: index out of range");
                return;
            }
            ItemInWorld worldItem = worldItems[index];
            ItemPile.RemoveItemFromPile(worldItem.item);
            RemoveItem(worldItem);
        }

        public void RemoveItem(GeneralItem item, bool shouldDestroy = false)
        {
            ItemInWorld worldItem = worldItems.Find(worldItem => worldItem.item == item);
            if (worldItem != null)
            {
                RemoveItem(worldItem, shouldDestroy);
            }
            else 
            {
                Debug.Log("Item not found");
            }
        }
        public void RemoveLastItem()
        {

        }

        public void DestroyItemPile()
        {
            Destroy(gameObject);
        }

        public void AddRigidBodyToItems()
        {
            if (isRigidBody) { return; }
            isRigidBody = true;
            foreach (ItemInWorld itemPrefab in worldItems)
            {
                GameObject itemGO = itemPrefab.gameObject;
                itemGO.GetComponent<BoxCollider>().isTrigger = false;
                itemGO.AddComponent<Rigidbody>();
            }
        }

        public void RemoveRigidBodyFromItems() 
        {
            isRigidBody = false;
            foreach (ItemInWorld itemPrefab in worldItems)
            {
                GameObject itemGO = itemPrefab.gameObject;
                BoxCollider collider;
                if (!itemGO.TryGetComponent<BoxCollider>(out collider))
                {
                    collider = itemGO.GetComponentInChildren<BoxCollider>();
                }
                if (collider != null) collider.isTrigger = true;
                if (itemGO.TryGetComponent<Rigidbody>(out Rigidbody rb)) { Destroy(rb); }
            }
        }

        public void ChangeParent(Transform newParent)
        {
            Debug.Log("Reparenting " + ItemPile.ToString() + " to " + newParent.ToString());
            transform.SetParent(newParent, true);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            if (ItemPile.IsPileUniqueItem)
            {
                ItemInWorld itemInWorld = worldItems[0];
                itemInWorld.transform.SetLocalPositionAndRotation(itemInWorld.item.InHandPosition, itemInWorld.item.InHandRotation);
            }
        }
    }
}