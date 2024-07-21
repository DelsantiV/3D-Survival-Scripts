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
}
