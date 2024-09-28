using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemBaseData")]
public class Item_General_SO : ScriptableObject
{

    new public string name;
    public string icon_path;
    public string prefab_path;
    public string item_info;
    public string item_class;
    public float weight;
    public float bulk;

    public string classProperties;

    public Sprite iconInInventory;
    public GameObject itemPrefab;


    public void Initialize()
    {
        LoadIcon();
        LoadPrefab();
    }

    public override string ToString()
    {
        return name + " ("+item_class+")";
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
