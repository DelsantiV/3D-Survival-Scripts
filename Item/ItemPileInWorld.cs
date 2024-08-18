using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPileInWorld : MonoBehaviour
{
    private ItemPile itemPile;

    public void SpawnItemPile(ItemPile itemPile, Vector3 spawnPosition)
    {
        if (itemPile == null)
        {

        }
    }
}

public class ItemPileSpawner
{
    public static void SpawnPile(ItemPile pile, Vector3 worldPosition)
    {
        if (pile != null)
        {
            ItemPileInWorld itemPileInWorld = new GameObject("Pile " + pile.ToString()).AddComponent<ItemPileInWorld>();
            itemPileInWorld.SpawnItemPile(pile, worldPosition);
        }
    }
}
