using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItemInfos
{
    public GeneralItem item;
    public int itemAmount;
    //public bool canSpoil;

    public InventoryItemInfos()
    {
    }

    public InventoryItemInfos(GeneralItem item, int itemAmount)
    {
        this.item = item;
        this.itemAmount = itemAmount;
    }

}
