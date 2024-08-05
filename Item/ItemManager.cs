using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemManager
{
    private static List<GeneralItem> _items = new List<GeneralItem>();
    private static Dictionary<string, GeneralItem> _allItemsByName = new Dictionary<string, GeneralItem>();
    private static List<Item_General_SO> _allItemsSO = new List<Item_General_SO>();

    private static string iconTemplateAddress = "IconInInventoryTemplate.prefab";
    public static ItemInInventory itemUITemplate 
    {
        get
        {
            if (itemUITemplate == null)
            {
                Debug.Log("Reloading Item UI Teemplate...");
                itemUITemplate = Addressables.LoadAssetAsync<GameObject>(iconTemplateAddress).WaitForCompletion().AddComponent<ItemInInventory>();
            }
            return itemUITemplate;
        }
        private set
        {
            itemUITemplate = value;
        } 
    }

    public static IEnumerator InitializeItemManager(Dictionary<string, GeneralItem> allItemsByName, List<Item_General_SO> allItemsSO)
    {
        _allItemsByName = allItemsByName;
        _items = _allItemsByName.Values.ToList();
        _allItemsSO = allItemsSO;
        AsyncOperationHandle<GameObject> itemIconHandle = Addressables.LoadAssetAsync<GameObject>(iconTemplateAddress);
        itemIconHandle.Completed += OnItemIconLoaded;
        yield return itemIconHandle;
    }

    public static void OnItemIconLoaded(AsyncOperationHandle<GameObject> itemIconHandle)
    {
        if (itemIconHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject itemUIGO = itemIconHandle.Result;
            itemUITemplate = itemUIGO.AddComponent<ItemInInventory>();
        }
        Addressables.Release(itemIconHandle);
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
