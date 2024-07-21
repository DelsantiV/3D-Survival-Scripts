using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemBaseData")]
public class Item_General_SO : ScriptableObject
{

    new public string name;
    public string icon_path;
    public string prefab_path;
    public int max_stacksize;
    public string item_info;
    public string item_class;

    public Sprite iconInInventory;
    public GameObject itemPrefab;
    public int maxStackSize
    {
        get { return max_stacksize; }
    }


    public void OnInitialize()
    {
        RunTimeLoader.LoadIcon(this);
    }

}
