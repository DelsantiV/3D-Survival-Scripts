using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

public class ItemLoader
{
    private Dictionary<string, ItemInInventory> allItems;
    private string[] allItemNames;
    public static UnityEvent Ready;
    Assembly asm = typeof(ItemInInventory).Assembly;

    public ItemLoader()
    {
        Ready = new UnityEvent();
        allItems = new Dictionary<string, ItemInInventory>();
    }

    public IEnumerator LoadItemsJSONFromMemory()
    {
        Debug.Log("Start retrieving items json...");
        AsyncOperationHandle<IList<IResourceLocation>> itemsJsonLocations = Addressables.LoadResourceLocationsAsync("Items", typeof(TextAsset));
        yield return itemsJsonLocations;
        Debug.Log(itemsJsonLocations.Status.ToString());
        Debug.Log(itemsJsonLocations.Result.Count + " items found in assets");

        var loadOpsBasic = new List<AsyncOperationHandle>(itemsJsonLocations.Result.Count);
        foreach (IResourceLocation loc in itemsJsonLocations.Result)
        {
            AsyncOperationHandle<TextAsset> jsonFileHandle = Addressables.LoadAssetAsync<TextAsset>(loc);
            jsonFileHandle.Completed += OnItemJsonDataLoaded;
            loadOpsBasic.Add(jsonFileHandle);
        }
        yield return Addressables.ResourceManager.CreateGenericGroupOperation(loadOpsBasic, true);
        allItemNames = allItems.Keys.ToArray();
        foreach (string key in allItemNames)
        {
            Debug.Log(key + allItems[key].itemSO.item_info);
        }
    }

    private void OnItemJsonDataLoaded(AsyncOperationHandle<TextAsset> jsonFileHandle)
    {
        if (jsonFileHandle.Status == AsyncOperationStatus.Succeeded)
        {
            CreateItem(jsonFileHandle.Result);
        }
    }

    private void CreateItem(TextAsset jsonFile)
    {
        JObject jsonParsedFile = JObject.Parse(jsonFile.text);
        bool shouldLoad = (bool) jsonParsedFile["should_load_in_game"];
        if (shouldLoad)
        {
            LoadItemFromJObject(jsonParsedFile);
        }
    }

    private void LoadItemFromJObject(JObject jsonParsedFile)
    {
        Item_General_SO itemBaseData = (Item_General_SO) ScriptableObject.CreateInstance(typeof(Item_General_SO));
        itemBaseData = jsonParsedFile["baseinfos"].ToObject<Item_General_SO>();
        Debug.Log("Loaded " + itemBaseData.name + " with max stack size " + itemBaseData.max_stacksize);
        var item = Activator.CreateInstance(asm.GetType(itemBaseData.name));
        RegisterItem(item as ItemInInventory, itemBaseData.name);
    }

    private void RegisterItem(ItemInInventory item, string itemName)
    {
        allItems.Add(itemName, item);
    }
}
