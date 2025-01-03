using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GoTF.Content
{
    public class ItemPileInWorld : MonoBehaviour
    {
        public ItemPile ItemPile { get; private set; }
        private Dictionary<GeneralItem, ItemInWorld> itemsPrefabs = new();
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
        /// <summary>
        /// Spawns the item pile at the given position.
        /// </summary>
        /// <param name="itemPile" The ItemPile to spawn></param>
        /// <param name="spawnPosition" The spawning position in world></param>
        /// <param name="isRigidBody" Should the items have a RigidBody></param>
        public void SpawnItemPile(ItemPile itemPile, Vector3 spawnPosition, bool isRigidBody = true)
        {
            this.ItemPile = itemPile;
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
        /// Spawns the item pile as a child of the target Transform
        /// </summary>
        /// <param name="itemPile" The ItemPile to spawn></param>
        /// <param name="targetTransform" The target Transform></param>
        /// <param name="isRigidBody" Should the items have a RigidBody></param>
        public void SpawnItemPile(ItemPile itemPile, Transform targetTransform, bool isRigidBody = true)
        {
            this.ItemPile = itemPile;
            this.isRigidBody = isRigidBody;
            transform.SetParent(targetTransform);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            if (itemPile != null)
            {
                foreach (GeneralItem item in ItemPile.ItemsInPile)
                {
                    SpawnIndividualItem(item, isRigidBody, out float previousItemHeight, pileHeight);
                    pileHeight += previousItemHeight;
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
        public void SpawnItemPile(ItemPile itemPile, Transform targetTransform, Vector3 spawnPosition, Quaternion spawnRotation, bool isRigidBody = true)
        {
            this.ItemPile = itemPile;
            this.isRigidBody = isRigidBody;
            transform.SetParent(targetTransform);
            transform.SetLocalPositionAndRotation(spawnPosition, spawnRotation);
            if (itemPile != null)
            {
                foreach (GeneralItem item in ItemPile.ItemsInPile)
                {
                    SpawnIndividualItem(item, isRigidBody, out float previousItemHeight, pileHeight);
                    pileHeight += previousItemHeight;
                }
            }
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
                    itemPrefab.transform.localPosition = Vector3.up * height;
                    ItemInWorld itemInWorld = itemPrefab.AddComponent<ItemInWorld>();
                    itemInWorld.item = item;
                    if (isRigidBody) { itemPrefab.AddComponent<Rigidbody>(); }
                    else { prefabCollider.isTrigger = true; }
                    itemsPrefabs.Add(item, itemInWorld);
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
                    itemPrefab.transform.localPosition = Vector3.up * height;
                    ItemInWorld itemInWorld = itemPrefab.AddComponent<ItemInWorld>();
                    itemInWorld.item = item;
                    if (isRigidBody) { itemPrefab.AddComponent<Rigidbody>(); }
                    else { prefabCollider.isTrigger = true; }
                    itemsPrefabs.Add(item, itemInWorld);
                }
            }
        }

        public void AddItem(GeneralItem item, bool applyHeight = true)
        {
            SpawnIndividualItem(item, isRigidBody, out pileHeight, applyHeight ? pileHeight : 0);
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
        public void RemoveItem(GeneralItem item)
        {
            if (!itemsPrefabs.TryGetValue(item, out ItemInWorld itemToRemove))
            {
                Debug.Log("Removing " + item.ItemName + " from pile " + ItemPile.ToString() + " failed: no such item found");
                return;
            }
            itemsPrefabs.Remove(item);
            // Should recalculate pile Height

            // Suppose that item is already removed from itemPile

            // If Pile is now empty, destroy whole GameObject

            if (itemsPrefabs.Count == 0)
            {
                Destroy(gameObject);
                return;
            }

            Destroy(itemToRemove.gameObject);
        }

        public void RemoveItem(int index)
        {
            if (index < 0 || index >= itemsPrefabs.Count)
            {
                Debug.Log("Removing item number " + index + " from pile " + ItemPile.ToString() + " failed: index out of range");
                return;
            }
            RemoveItem(itemsPrefabs.Keys.ToList()[index]);
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
            foreach (ItemInWorld itemPrefab in itemsPrefabs.Values)
            {
                GameObject itemGO = itemPrefab.gameObject;
                itemGO.GetComponent<BoxCollider>().isTrigger = false;
                itemGO.AddComponent<Rigidbody>();
            }
        }

        public void ChangeParent(Transform newParent)
        {
            transform.SetParent(newParent, false);
        }
    }
}