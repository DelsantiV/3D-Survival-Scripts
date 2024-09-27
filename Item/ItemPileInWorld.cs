using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPileInWorld : MonoBehaviour
{
    private ItemPile itemPile;
    private List<ItemInWorld> itemsPrefabs = new List<ItemInWorld>();

    public List<GeneralItem> ItemsInPile {  get { return itemPile.ItemsInPile; } }
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
            int height = 0;
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                SpawnIndividualItem(item, applyRigidBody, height);
                height++;
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
            float currentItemHeight;
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                SpawnIndividualItem(item, applyRigidBody, out currentItemHeight, previousItemHeight);
                previousItemHeight = currentItemHeight;
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
                itemsPrefabs.Add(itemInWorld);
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
                itemsPrefabs.Add(itemInWorld);
            }
        }
    }

    public void AddItem(GeneralItem item, bool applyRigidBody)
    {
        SpawnIndividualItem(item, applyRigidBody);
    }
}