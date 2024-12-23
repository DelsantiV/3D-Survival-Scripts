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
using GoTF.Content;
using GoTF.Utilities;
using System.Reflection;

namespace GoTF.GameLoading
{
    public class ItemLoader
    {
        private Dictionary<string, Item_General_SO> allItemsSOByName;
        private Dictionary<string, GeneralItem> allItemsByName;
        public static UnityEvent Ready;
        public static List<string> allPrefabsLocations;
        public static List<string> allIconsLocations;
        public static string[] allItemNames;
        public static Dictionary<string, Type> allItemTypes;
        private List<string> itemsJsonKeys = new List<string>() { "JSON", "Items" };
        private List<string> itemsPrefabsKeys = new List<string>() { "Prefabs", "Items" };
        private List<string> itemsIconsKeys = new List<string>() { "Icons", "Items" };
        private static readonly Assembly asm = Assembly.GetAssembly(typeof(GeneralItem));

        public ItemLoader()
        {
            Ready = new UnityEvent();
            allItemsSOByName = new();
            allItemTypes = new();
            allItemsByName = new();
        }

        public IEnumerator LoadItems()
        {
            Task ItemClassesRegistering = new Task(RegisterItemClasses());
            yield return ItemClassesRegistering;

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
                Addressables.MergeMode.Intersection);
            yield return itemsLoading;
            Task itemManagerInitialization = new Task(ItemManager.InitializeItemManager(allItemsSOByName));
            itemManagerInitialization.Finished += FireAssetsReady;
            yield return itemManagerInitialization;
            Addressables.Release(allPrefabsLocationsLoading);
            Addressables.Release(allIconsLocationsLoading);
            Addressables.Release(itemsLoading);
        }

        public IEnumerator RegisterItemClasses()
        {
            //var result = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(GeneralItem))).ToList(); // Get all item classes
            allItemTypes.Add("Weapon", typeof(WeaponItem));
            allItemTypes.Add("Food", typeof(FoodItem));
            yield return allItemTypes;
        }

        public void FireAssetsReady(bool manual)
        {
            Ready.Invoke();
        }

        private void CreateItem(TextAsset jsonFile)
        {
            JObject jsonParsedFile = JObject.Parse(jsonFile.text);
            bool shouldLoad = (bool)jsonParsedFile["should_load_in_game"];
            if (shouldLoad)
            {
                LoadItemFromJObject(jsonParsedFile);
            }
        }

        private void LoadItemFromJObject(JObject jsonParsedFile)
        {
            Item_General_SO itemBaseData = (Item_General_SO) ScriptableObject.CreateInstance(typeof(Item_General_SO));
            //Debug.Log(jsonParsedFile["baseinfos"].ToString());
            JsonConvert.PopulateObject(jsonParsedFile["baseinfos"].ToString(), itemBaseData);
            itemBaseData.Initialize();
            itemBaseData.classProperties = jsonParsedFile["class_properties"].ToString();
            Debug.Log("Loaded " + itemBaseData.name + " from class " + itemBaseData.item_class);   
            allItemsSOByName.TryAdd(itemBaseData.name, itemBaseData);
            allItemTypes.TryGetValue(itemBaseData.item_class_name, out Type itemType);
            var item = (GeneralItem) Activator.CreateInstance(itemType);
            item.Initialize(itemBaseData);
            allItemsByName.TryAdd(itemBaseData.name, item);
        }
    }
}
