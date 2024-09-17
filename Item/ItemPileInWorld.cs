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

    private void SpawnIndividualItem(GeneralItem item, bool applyRigidBody, float height = 0)
    {
        if (item != null)
        {
            if (item.ItemPrefab != null)
            {
                GameObject itemPrefab = Instantiate(item.ItemPrefab, transform);
                itemPrefab.transform.localPosition = Vector3.up*height;
                ItemInWorld itemInWorld = itemPrefab.AddComponent<ItemInWorld>();
                itemInWorld.item = item;
                if (applyRigidBody) { itemPrefab.AddComponent<Rigidbody>(); }
                itemsPrefabs.Add(itemInWorld);
            }
        }
    }

    public void AddItem(GeneralItem item, bool applyRigidBody)
    {
        SpawnIndividualItem(item, applyRigidBody);
    }
}