using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftingCost
{
    public GeneralItem item;
    public int itemAmount;
    public bool isTool;

    public CraftingCost()
    {
    }

    public CraftingCost(GeneralItem item, int itemAmount, bool isTool)
    {
        this.item = item;
        this.itemAmount = itemAmount;
        this.isTool = isTool;
    }
}
