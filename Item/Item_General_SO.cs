using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GoTF.GameLoading;

namespace GoTF.Content
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ItemBaseData")]
    public class Item_General_SO : ScriptableObject
    {

        new public string name;
        public string icon_path;
        public string prefab_path;
        public string item_info;
        public string item_class_name;
        public float weight;
        public float bulk;
        public Vector3 hand_position;
        public Vector3 hand_rotation;
        public Vector3 pile_position;
        public Vector3 pile_rotation;

        public string classProperties;

        public Sprite iconInInventory;
        public GameObject itemPrefab;
        public Type item_class;


        public void Initialize()
        {
            LoadIcon();
            LoadPrefab();
            GetClass();
        }

        public override string ToString()
        {
            return name + " (" + item_class_name + ")";
        }

        public void GetClass()
        {
            if (ItemLoader.allItemTypes.TryGetValue(item_class_name, out item_class))
            {

            }
            else
            {
                Debug.Log("Failed finding class " + item_class + " for item " + name);
            }
        }

        public void LoadIcon()
        {
            if (ItemLoader.allIconsLocations.Contains("Assets/Data/Icons/" + icon_path))
            {
                AsyncOperationHandle<Sprite> spriteLoadOpHandle = Addressables.LoadAssetAsync<Sprite>("Icons/" + icon_path);
                spriteLoadOpHandle.Completed += delegate { OnLoadIconComplete(spriteLoadOpHandle); };
            }
        }

        private void OnLoadIconComplete(AsyncOperationHandle<Sprite> spriteLoadOpHandle)
        {
            if (spriteLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
            {
                iconInInventory = spriteLoadOpHandle.Result;
            }
        }

        public void LoadPrefab()
        {
            if (ItemLoader.allPrefabsLocations.Contains("Assets/Data/Prefabs/" + prefab_path))
            {
                AsyncOperationHandle<GameObject> prefabLoadOpHandle = Addressables.LoadAssetAsync<GameObject>("Prefabs/" + prefab_path);
                prefabLoadOpHandle.Completed += delegate { OnLoadPrefabComplete(prefabLoadOpHandle); };
            }
            else
            {
                Debug.Log("No prefab found for " + name);
            }
        }

        private void OnLoadPrefabComplete(AsyncOperationHandle<GameObject> prefabLoadOpHandle)
        {
            if (prefabLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
            {
                itemPrefab = prefabLoadOpHandle.Result;
            }
        }

    }
}
