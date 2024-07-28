using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

public class ItemLoader
{
    private Dictionary<string, ItemInInventory> allItems;
    private string[] allItemNames;
    public static UnityEvent Ready;
    public static List<string> allPrefabsLocations;
    public static List<string> allIconsLocations;
    Assembly asm = typeof(ItemInInventory).Assembly;

    public ItemLoader()
    {
        Ready = new UnityEvent();
        allItems = new Dictionary<string, ItemInInventory>();
    }

    public IEnumerator LoadItemsJSONFromMemory()
    {
        Debug.Log("Start retrieving items json locations...");
        AsyncOperationHandle<IList<IResourceLocation>> itemsJsonLocations = Addressables.LoadResourceLocationsAsync("Items", typeof(TextAsset));
        yield return itemsJsonLocations;
        Debug.Log(itemsJsonLocations.Status.ToString());
        Debug.Log(itemsJsonLocations.Result.Count + " items found in assets");

        Debug.Log("Start retrieving items prefabs locations...");
        AsyncOperationHandle<IList<IResourceLocation>> allPrefabsLocationsLoading = Addressables.LoadResourceLocationsAsync("Prefabs/Items", typeof(GameObject));
        yield return allPrefabsLocationsLoading;
        Debug.Log(allPrefabsLocationsLoading.Status.ToString());
        allPrefabsLocations = allPrefabsLocationsLoading.Result.ToList().ConvertAll(address => address.ToString());
        Debug.Log(allPrefabsLocationsLoading.Result.Count + " prefabs found in assets");

        Debug.Log("Start retrieving items icons locations...");
        AsyncOperationHandle<IList<IResourceLocation>> allIconsLocationsLoading = Addressables.LoadResourceLocationsAsync("Icons/Items", typeof(Sprite));
        yield return allIconsLocationsLoading;
        Debug.Log(allIconsLocationsLoading.Status.ToString());
        allIconsLocations = allIconsLocationsLoading.Result.ToList().ConvertAll(address => address.ToString());
        Debug.Log(allIconsLocations.Count + " icons found in assets");

        var loadOpsBasic = new List<AsyncOperationHandle>(itemsJsonLocations.Result.Count);
        foreach (IResourceLocation loc in itemsJsonLocations.Result)
        {
            AsyncOperationHandle<TextAsset> jsonFileHandle = Addressables.LoadAssetAsync<TextAsset>(loc);
            jsonFileHandle.Completed += OnItemJsonDataLoaded;
            loadOpsBasic.Add(jsonFileHandle);
        }
        yield return Addressables.ResourceManager.CreateGenericGroupOperation(loadOpsBasic, true);
        allItemNames = allItems.Keys.ToArray();
        ItemManager.InitializeItemManager(allItems);
        Ready.Invoke();
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
        Debug.Log(jsonParsedFile["baseinfos"].ToString());
        JsonConvert.PopulateObject(jsonParsedFile["baseinfos"].ToString(), itemBaseData);
        Debug.Log("Loaded " + itemBaseData.name + " from class " + itemBaseData.item_class);
        var item = Activator.CreateInstance(asm.GetType(itemBaseData.item_class));
        RegisterItem(item as ItemInInventory, itemBaseData.name);
    }

    private void RegisterItem(ItemInInventory item, string itemName)
    {
        allItems.Add(itemName, item);
    }
}
