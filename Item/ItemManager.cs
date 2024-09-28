using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Reflection;

public class ItemManager
{
    private static Dictionary<string, Item_General_SO> _allItemsSOByName = new Dictionary<string, Item_General_SO>();
    private static List<Item_General_SO> _allItemsSO = new List<Item_General_SO>();
    private static List<string> _allItemsNames = new List<string>();

    private static Assembly asm = typeof(GeneralItem).Assembly;

    private static string itemInfoAddress = "ItemInfoTemplate.prefab";
    private static string pileIconTemplateAddress = "PileIconInInventoryTemplate.prefab";


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
    }

    private static GameObject pileIconTemplate;
    public static ItemPileInInventory PileIconTemplate
    {
        get
        {
            if (pileIconTemplate == null)
            {
                Debug.Log("Reloading Pile Icon Template...");
                pileIconTemplate = Addressables.LoadAssetAsync<GameObject>(pileIconTemplateAddress).WaitForCompletion();
            }
            return pileIconTemplate.GetComponent<ItemPileInInventory>();
        }
    }

    public static IEnumerator InitializeItemManager(Dictionary<string, Item_General_SO> allItemsSOByName)
    {
        _allItemsSOByName = allItemsSOByName;
        _allItemsSO = allItemsSOByName.Values.ToList();
        AsyncOperationHandle<GameObject> itemInfoHandle = Addressables.LoadAssetAsync<GameObject>(itemInfoAddress);
        AsyncOperationHandle<GameObject> pileIconTemplateHandle = Addressables.LoadAssetAsync<GameObject>(pileIconTemplateAddress);
        itemInfoHandle.Completed += delegate { OnGameObjectLoaded(itemInfoHandle, out itemInfoTemplate); };
        pileIconTemplateHandle.Completed += delegate { OnGameObjectLoaded(pileIconTemplateHandle, out pileIconTemplate); };
        yield return pileIconTemplateHandle;
    }

    public static void OnGameObjectLoaded(AsyncOperationHandle<GameObject> loadHandle, out GameObject result)
    {
        result = null;
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            result = loadHandle.Result;
        }
        Addressables.Release(loadHandle);
    }

    public static void OnSpriteLoaded(AsyncOperationHandle<Sprite> loadHandle, out Sprite result)
    {
        result = null;
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            result = loadHandle.Result;
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

    public static GeneralItem GenerateItemfromItemSO(Item_General_SO itemSO)
    {
        //var item = Activator.CreateInstance(asm.GetType(itemSO.item_class));
        //(item as GeneralItem).ItemSO = itemSO;
        return new GeneralItem(itemSO);
    }
    public static GeneralItem GetItemByName(string itemName)
    {
        if (_allItemsSOByName.ContainsKey(itemName)) { return GenerateItemfromItemSO(_allItemsSOByName[itemName]); }
        else
        {
            Debug.Log("No item named " + itemName);
            return null;
        }
    }

    public static GeneralItem GetItemByID(int id)
    {
        if (id > 0 && id < _allItemsSO.Count) { return GenerateItemfromItemSO(_allItemsSO[id]);}
        else 
        {
            Debug.Log("Item ID " + id + " is not valid");
            return null; 
        }
    }

}
