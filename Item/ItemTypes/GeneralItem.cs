using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEditor.Progress;

public class GeneralItem
{
    public Item_General_SO ItemSO { get; protected set; }

    public event Action<PlayerManager, GeneralItem> OnItemUsed;

    public GameObject ItemPrefab { get { return ItemSO.itemPrefab; } }
    public Sprite ItemSprite { get { return ItemSO.iconInInventory; } }
    public string ItemName { get { return ItemSO.name; } }

    /// <summary>
    /// Item weight in kg
    /// </summary>
    public float Weight { get { return ItemSO.weight; } }

    /// <summary>
    /// 1/Bulk = number of items a normal human can carry
    /// </summary>
    public float Bulk { get { return ItemSO.bulk; } }


    public GeneralItem(Item_General_SO itemSO)
    {
        ItemSO = itemSO;
    }

    public virtual void UseItem(PlayerManager player, ItemPileInInventory pileUI)
    {

    }

    public virtual void EquipItem()
    {

    }
    public virtual void SpawnInWorld(Vector3 spawnPosition)
    {

    }

    public void Action(PlayerManager player)
    {

    }
}
