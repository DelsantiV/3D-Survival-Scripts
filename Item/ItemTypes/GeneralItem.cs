using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class GeneralItem
{
    public Item_General_SO ItemSO { get; protected set; }

    public event Action<PlayerManager, GeneralItem> OnItemUsed;

    public GameObject ItemPrefab { get { return ItemSO.itemPrefab; } }
    public Sprite ItemSprite { get { return ItemSO.iconInInventory; } }
    public int StackSize { get { return ItemSO.maxStackSize; } }
    public string ItemName { get { return ItemSO.name; } }

    public GeneralItem(Item_General_SO itemSO)
    {
        ItemSO = itemSO;
    }

    public virtual void InitializeItemInInventory(ItemInInventory itemUI, int amount)
    {
        itemUI.SetItem(this, amount : amount);
        itemUI.Initialize();
        itemUI.amountOfItem = amount;
    }

    public virtual void UseItem(PlayerManager player, ItemInInventory itemUI)
    {

    }

    public virtual void EquipItem()
    {

    }
}
