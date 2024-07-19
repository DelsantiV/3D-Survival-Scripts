using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItemInfos
{
    public Item_General_SO itemSO;
    public int itemAmount;
    //public bool canSpoil;

    public InventoryItemInfos()
    {
    }

    public InventoryItemInfos(Item_General_SO itemSO, int itemAmount)
    {
        this.itemSO = itemSO;
        this.itemAmount = itemAmount;
    }

}
