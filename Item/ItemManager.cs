using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager
{
    private static List<GeneralItem> _items = new List<GeneralItem>();
    private static Dictionary<string, GeneralItem> _allItemsByName = new Dictionary<string, GeneralItem>();
    private static List<Item_General_SO> _allItemsSO = new List<Item_General_SO>();

    public static void InitializeItemManager(Dictionary<string, GeneralItem> allItemsByName, List<Item_General_SO> allItemsSO)
    {
        _allItemsByName = allItemsByName;
        _items = _allItemsByName.Values.ToList();
        _allItemsSO = allItemsSO;
    }

    public static Item_General_SO GetItemSOByName(string itemName)
    {
        Item_General_SO item = _allItemsSO.Find(item => item.name == itemName);
        if (item != null) { return item; }
        else
        {
            Debug.Log("No item named " + itemName);
            return null;
        }
  
    }
    public static GeneralItem GetItemByName(string itemName)
    {
        if (_allItemsByName.ContainsKey(itemName)) { return _allItemsByName[itemName]; }
        else
        {
            Debug.Log("No item named " + itemName);
            return null;
        }
    }

    public static GeneralItem GetItemByID(int id)
    {
        if (id > 0 && id < _items.Count) { return _items[id];}
        else 
        {
            Debug.Log("Item ID " + id + " is not valid");
            return null; 
        }
    }
}
