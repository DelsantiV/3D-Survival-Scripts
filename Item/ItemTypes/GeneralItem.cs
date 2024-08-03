using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class GeneralItem
{
    public Item_General_SO ItemSO { get; }

    public event Action<PlayerManager, GeneralItem> OnItemUsed;

    public GameObject ItemPrefab { get { return ItemSO.itemPrefab; } }
    public Sprite ItemSprite { get { return ItemSO.iconInInventory; } }
    public int StackSize { get { return ItemSO.maxStackSize; } }
    public string ItemName { get { return ItemSO.name; } }

    public virtual void InitializeItem(ItemInInventory itemUI)
    {
        
    }

    public virtual void UseItem(PlayerManagerV2 player, ItemInInventory itemUI)
    {

    }

    public virtual void EquipItem()
    {

    }
}
