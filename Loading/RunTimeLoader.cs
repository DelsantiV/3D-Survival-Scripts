using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public static class RunTimeLoader
{
    public static void LoadIcon(Item_General_SO iconReceiver)
    {
        AsyncOperationHandle<Sprite> spriteLoadOpHandle = Addressables.LoadAssetAsync<Sprite>(iconReceiver.icon_path);
        spriteLoadOpHandle.Completed += delegate { OnLoadIconComplete(spriteLoadOpHandle, iconReceiver); };
    }

    private static void OnLoadIconComplete(AsyncOperationHandle<Sprite> spriteLoadOpHandle, Item_General_SO iconReceiver)
    {
        if (spriteLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
        {
            iconReceiver.iconInInventory = spriteLoadOpHandle.Result;
        }
    }

    public static void LoadPrefab(Item_General_SO prefabReceiver)
    {
        AsyncOperationHandle<GameObject> prefabLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(prefabReceiver.prefab_path);
        prefabLoadOpHandle.Completed += delegate { OnLoadPrefabComplete(prefabLoadOpHandle, prefabReceiver); };
    }

    private static void OnLoadPrefabComplete(AsyncOperationHandle<GameObject> prefabLoadOpHandle, Item_General_SO iconReceiver)
    {
        if (prefabLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
        {
            iconReceiver.itemPrefab = prefabLoadOpHandle.Result;
        }
    }

}
