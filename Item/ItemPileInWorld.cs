using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPileInWorld : MonoBehaviour
{
    private ItemPile itemPile;

    public void SpawnItemPile(ItemPile itemPile, Vector3 spawnPosition)
    {
        this.itemPile = itemPile;
        transform.position = spawnPosition;
        if (itemPile != null)
        {
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                SpawnIndividualItem(item);
            }
        }
    }

    public void SpawnItemPile(ItemPile itemPile, Transform targetTransform)
    {
        this.itemPile = itemPile;
        transform.SetParent(targetTransform);
        if (itemPile != null)
        {
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                SpawnIndividualItem(item);
            }
        }
    }

    private void SpawnIndividualItem(GeneralItem item)
    {
        if (item != null)
        {
            if (item.ItemPrefab != null)
            {
                GameObject itemPrefab = Instantiate(item.ItemPrefab, this.transform);
                itemPrefab.AddComponent<ItemInWorld>();
                itemPrefab.AddComponent<Rigidbody>();
                itemPrefab.GetComponent<ItemInWorld>().item = item;
            }
        }
    }
}