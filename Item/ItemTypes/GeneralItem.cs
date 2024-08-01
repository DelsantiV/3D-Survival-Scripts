using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralItem : IItem
{
    public Item_General_SO itemSO { get; }
    public ItemInInventory itemUI { get; private set; }
    protected ItemSlot slot;

    public event Action<PlayerManager, IItem> OnItemUsed;

    public virtual void InitializeItem(ItemInInventory itemUI)
    {
        this.itemUI = itemUI;
        this.slot = itemUI.slot;
    }

    public virtual void UseItem(PlayerManagerV2 player)
    {

    }

    public virtual void EquipItem(PlayerManagerV2 player)
    {

    }
}
