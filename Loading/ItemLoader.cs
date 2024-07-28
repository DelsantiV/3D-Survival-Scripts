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

public class ItemLoader
{
    private Dictionary<string, ItemInInventory> allItems;
    private string[] allItemNames;
    public static UnityEvent Ready;

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
        Debug.Log(shouldLoad);
    }
}
