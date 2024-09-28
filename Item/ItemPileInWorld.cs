using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemPileInWorld : MonoBehaviour
{
    private ItemPile itemPile;
    private Dictionary<GeneralItem, ItemInWorld> itemsPrefabs = new();
    public List<GeneralItem> ItemsInPile {  get { return itemPile.ItemsInPile; } }
    private float pileHeight;
    public float Weight
    {
        get { return itemPile.Weight; }
    }

    public float Bulk
    {
        get { return itemPile.Bulk; }
    }

    public void SpawnItemPile(ItemPile itemPile, Vector3 spawnPosition, bool applyRigidBody = true)
    {
        this.itemPile = itemPile;
        transform.position = spawnPosition;
        if (itemPile != null)
        {
            pileHeight = 0;
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                SpawnIndividualItem(item, applyRigidBody, pileHeight);
                pileHeight += 1;
            }
        }
    }

    public void SpawnItemPile(ItemPile itemPile, Transform targetTransform, bool applyRigidBody = true)
    {
        this.itemPile = itemPile;
        transform.SetParent(targetTransform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (itemPile != null)
        {
            float previousItemHeight = 0;
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                SpawnIndividualItem(item, applyRigidBody, out pileHeight, previousItemHeight);
                previousItemHeight = pileHeight;
            }
        }
    }

    private void SpawnIndividualItem(GeneralItem item, bool applyRigidBody, out float prefabHeight, float height = 0)
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
                if (applyRigidBody) { itemPrefab.AddComponent<Rigidbody>(); }
                else { prefabCollider.isTrigger = true;}
                itemsPrefabs.Add(item, itemInWorld);
            }
        }
    }
    private void SpawnIndividualItem(GeneralItem item, bool applyRigidBody, float height = 0)
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
                if (applyRigidBody) { itemPrefab.AddComponent<Rigidbody>(); }
                else { prefabCollider.isTrigger = true; }
                itemsPrefabs.Add(item, itemInWorld);
            }
        }
    }

    public void AddItem(GeneralItem item, bool applyRigidBody = false, bool applyHeight = true)
    {
        SpawnIndividualItem(item, applyRigidBody, out pileHeight, applyHeight ? pileHeight : 0);
    }

    public void RemoveItem(GeneralItem item)
    {
        ItemInWorld itemToRemove;
        if (!itemsPrefabs.TryGetValue(item, out itemToRemove))
        {
            Debug.Log("Removing " + item.ItemName + " from pile " + itemPile.ToString() + " failed: no such item found");
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
            Debug.Log("Removing item number " + index + " from pile " + itemPile.ToString() + " failed: index out of range");
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
}