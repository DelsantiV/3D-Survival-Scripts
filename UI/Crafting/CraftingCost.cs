using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftingCost
{
    public Item_General_SO item;
    public int itemAmount;
    public bool isTool;

    public CraftingCost()
    {
    }

    public CraftingCost(Item_General_SO item, int itemAmount, bool isTool)
    {
        this.item = item;
        this.itemAmount = itemAmount;
        this.isTool = isTool;
    }
}
