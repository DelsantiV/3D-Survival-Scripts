using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    public Item_General_SO itemSO { get; }
    public event Action<PlayerManager, IItem> OnItemUsed;
    public void UseItem(PlayerManagerV2 player);

    public void EquipItem(PlayerManagerV2 player);
}
