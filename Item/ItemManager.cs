using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager
{
    private static List<ItemInInventory> _items = new List<ItemInInventory>();
    private static Dictionary<string, ItemInInventory> _allItemsByName = new Dictionary<string, ItemInInventory>();

    public static void InitializeItemManager(Dictionary<string, ItemInInventory> allItemsByName)
    {
        _allItemsByName = allItemsByName;
        _items = _allItemsByName.Values.ToList();
    }

    public static ItemInInventory GetItemByName(string itemName)
    {
        if (_allItemsByName.ContainsKey(itemName)) { return _allItemsByName[itemName]; }
        else 
        { 
            Debug.Log("No item named " + itemName); 
            return null; 
        }
    }

    public static ItemInInventory GetItemByID(int id)
    {
        if (id > 0 && id < _items.Count) { return _items[id];}
        else 
        {
            Debug.Log("Item ID " + id + " is not valid");
            return null; 
        }
    }
}
