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
    private Dictionary<string, Item_General_SO> allItemsSOByName;
    public static UnityEvent Ready;
    public static List<string> allPrefabsLocations;
    public static List<string> allIconsLocations;
    public static string[] allItemNames;
    private List<string> itemsJsonKeys = new List<string>() { "JSON", "Items" };
    private List<string> itemsPrefabsKeys = new List<string>() { "Prefabs", "Items" };
    private List<string> itemsIconsKeys = new List<string>() { "Icons", "Items" };

    public ItemLoader()
    {
        Ready = new UnityEvent();
        allItemsSOByName = new();
    }

    public IEnumerator LoadItems()
    {
        
        Debug.Log("Start retrieving items prefabs locations...");
        AsyncOperationHandle<IList<IResourceLocation>> allPrefabsLocationsLoading = Addressables.LoadResourceLocationsAsync(itemsPrefabsKeys, Addressables.MergeMode.Intersection, typeof(GameObject));
        yield return allPrefabsLocationsLoading;
        Debug.Log(allPrefabsLocationsLoading.Status.ToString());
        allPrefabsLocations = allPrefabsLocationsLoading.Result.ToList().ConvertAll(address => address.ToString());
        Debug.Log(allPrefabsLocationsLoading.Result.Count + " prefabs found in assets");

        Debug.Log("Start retrieving items icons locations...");
        AsyncOperationHandle<IList<IResourceLocation>> allIconsLocationsLoading = Addressables.LoadResourceLocationsAsync(itemsIconsKeys, Addressables.MergeMode.Intersection, typeof(Sprite));
        yield return allIconsLocationsLoading;
        Debug.Log(allIconsLocationsLoading.Status.ToString());
        allIconsLocations = allIconsLocationsLoading.Result.ToList().ConvertAll(address => address.ToString());
        Debug.Log(allIconsLocations.Count + " icons found in assets");

        AsyncOperationHandle<IList<TextAsset>> itemsLoading = Addressables.LoadAssetsAsync<TextAsset>(itemsJsonKeys,
            itemJSON => { CreateItem(itemJSON); },
            Addressables.MergeMode.Intersection) ;
        yield return itemsLoading;
        Task itemManagerInitialization = new Task(ItemManager.InitializeItemManager(allItemsSOByName));
        itemManagerInitialization.Finished += FireAssetsReady;
        yield return itemManagerInitialization;
        Addressables.Release(allPrefabsLocationsLoading);
        Addressables.Release(allIconsLocationsLoading);
        Addressables.Release(itemsLoading);
    }

    public void FireAssetsReady (bool manual)
    {
        Ready.Invoke();
    }

    /* Old way of loading items
    public IEnumerator LoadItemsJSONFromMemory()
    {
        Debug.Log("Start retrieving items json locations...");
        AsyncOperationHandle<IList<IResourceLocation>> itemsJsonLocations = Addressables.LoadResourceLocationsAsync(itemsJsonKeys, Addressables.MergeMode.Intersection, typeof(TextAsset)) ;
        yield return itemsJsonLocations;
        Debug.Log(itemsJsonLocations.Status.ToString());
        Debug.Log(itemsJsonLocations.Result.Count + " items found in assets");

        Debug.Log("Start retrieving items prefabs locations...");
        AsyncOperationHandle<IList<IResourceLocation>> allPrefabsLocationsLoading = Addressables.LoadResourceLocationsAsync("Prefabs", typeof(GameObject));
        yield return allPrefabsLocationsLoading;
        Debug.Log(allPrefabsLocationsLoading.Status.ToString());
        allPrefabsLocations = allPrefabsLocationsLoading.Result.ToList().ConvertAll(address => address.ToString());
        Debug.Log(allPrefabsLocationsLoading.Result.Count + " prefabs found in assets");

        Debug.Log("Start retrieving items icons locations...");
        AsyncOperationHandle<IList<IResourceLocation>> allIconsLocationsLoading = Addressables.LoadResourceLocationsAsync("Icons", typeof(Sprite));
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
        ItemManager.InitializeItemManager(allItems, allItemsSO);
        foreach (string prefabLoc in allPrefabsLocations)
        {
            Debug.Log(prefabLoc);
        }
        Ready.Invoke();
    }

    private void OnItemJsonDataLoaded(AsyncOperationHandle<TextAsset> jsonFileHandle)
    {
        if (jsonFileHandle.Status == AsyncOperationStatus.Succeeded)
        {
            CreateItem(jsonFileHandle.Result);
        }
    }
    */

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
        itemBaseData.Initialize();
        itemBaseData.classProperties = jsonParsedFile["class_properties"].ToString();
        Debug.Log("Loaded " + itemBaseData.name + " from class " + itemBaseData.item_class);
        allItemsSOByName.Add(itemBaseData.name, itemBaseData);
    }
}
