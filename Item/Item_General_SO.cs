using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items")]
public class Item_General_SO : ScriptableObject {

    public string itemName;
    public Sprite iconInInventory;
    public GameObject itemPrefab;
    public int maxStackSize;
    public bool isUsable;
    public string itemInfo;
    public ItemTypesEnum itemType;
}
