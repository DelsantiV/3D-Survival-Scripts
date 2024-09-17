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
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                SpawnIndividualItem(item, applyRigidBody);
            }
        }
    }

    public void SpawnItemPile(ItemPile itemPile, Transform targetTransform, bool applyRigidBody = true)
    {
        this.itemPile = itemPile;
        transform.SetParent(targetTransform);
        if (itemPile != null)
        {
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                SpawnIndividualItem(item, applyRigidBody);
            }
        }
    }

    private void SpawnIndividualItem(GeneralItem item, bool applyRigidBody)
    {
        if (item != null)
        {
            if (item.ItemPrefab != null)
            {
                GameObject itemPrefab = Instantiate(item.ItemPrefab, transform, false);
                itemPrefab.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
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