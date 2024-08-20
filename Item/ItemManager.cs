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
    private static string itemInfoAddress = "ItemInfoTemplate.prefab";
    private static string pileIconAddress = "ItemPile.png";

    private static ItemInInventory itemUITemplate;
    public static ItemInInventory ItemUITemplate
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
    private static GameObject itemInfoTemplate;
    public static GameObject ItemInfoTemplate
    {
        get
        {
            if (itemInfoTemplate == null)
            {
                Debug.Log("Reloading Item UI Template...");
                itemInfoTemplate = Addressables.LoadAssetAsync<GameObject>(itemInfoAddress).WaitForCompletion();
            }
            return itemInfoTemplate;
        }
        private set
        {
            itemInfoTemplate = value;
        }
    }
    private static Sprite pileIcon;
    public static Sprite PileIcon
    {
        get
        {
            if (pileIcon == null)
            {
                Debug.Log("Reloading Pile Icon Template...");
                pileIcon = Addressables.LoadAssetAsync<Sprite>(pileIconAddress).WaitForCompletion();
            }
            return pileIcon;
        }
        private set
        {
            pileIcon = value;
        }
    }

    public static IEnumerator InitializeItemManager(Dictionary<string, GeneralItem> allItemsByName, List<Item_General_SO> allItemsSO)
    {
        _allItemsByName = allItemsByName;
        _items = _allItemsByName.Values.ToList();
        _allItemsSO = allItemsSO;
        AsyncOperationHandle<GameObject> itemIconHandle = Addressables.LoadAssetAsync<GameObject>(iconTemplateAddress);
        AsyncOperationHandle<GameObject> itemInfoHandle = Addressables.LoadAssetAsync<GameObject>(iconTemplateAddress);
        AsyncOperationHandle<GameObject> pileIconHandle = Addressables.LoadAssetAsync<GameObject>(iconTemplateAddress);
        itemIconHandle.Completed += OnItemIconLoaded;
        itemInfoHandle.Completed += delegate { OnResultLoaded(itemInfoHandle, itemInfoTemplate); };
        pileIconHandle.Completed += delegate { OnResultLoaded(pileIconHandle, PileIcon); };
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
    public static void OnResultLoaded(AsyncOperationHandle<GameObject> loadHandle, Object receiver)
    {
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            receiver = loadHandle.Result;
        }
        Addressables.Release(loadHandle);
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
