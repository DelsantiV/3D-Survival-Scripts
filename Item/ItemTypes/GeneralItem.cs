using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEditor.Progress;

public class GeneralItem : ICarryable
{
    public Item_General_SO ItemSO { get; protected set; }

    public event Action<PlayerManager, GeneralItem> OnItemUsed;

    public GameObject ItemPrefab { get { return ItemSO.itemPrefab; } }
    public Sprite ItemSprite { get { return ItemSO.iconInInventory; } }
    public int StackSize { get { return ItemSO.maxStackSize; } }
    public string ItemName { get { return ItemSO.name; } }

    private string iconTemplateAddress = "IconInInventoryTemplate.prefab";

    public GeneralItem(Item_General_SO itemSO)
    {
        ItemSO = itemSO;
    }

    public virtual IEnumerator CreateItemInInventory(int amount, ItemSlot slot)
    {
        slot._isInOp = true;
        AsyncOperationHandle<GameObject> itemIconHandle = Addressables.LoadAssetAsync<GameObject>(iconTemplateAddress);
        itemIconHandle.Completed += delegate { OnItemIconLoaded(itemIconHandle, amount, slot); };
        yield return itemIconHandle;
    }

    public virtual void OnItemIconLoaded(AsyncOperationHandle<GameObject> itemIconHandle, int amount, ItemSlot slot)
    {
        if (itemIconHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject itemUIGO = UnityEngine.Object.Instantiate(itemIconHandle.Result, slot.transform.position, slot.transform.rotation);
            ItemInInventory itemUI = itemUIGO.AddComponent<ItemInInventory>();
            itemUI.SetItem(this);
            slot.SetItemToSlot(itemUI);
        }
        slot._isInOp = false;
        Addressables.Release(itemIconHandle);
    }

    public virtual ItemInInventory CreateItemInInventory()
    {
        ItemInInventory itemUI = ItemManager.itemUITemplate;
        itemUI.SetItem(this); 
        return itemUI;
    }

    public virtual void SetItemToSlot(ItemSlot slot)
    {
        slot.SetItemToSlot(CreateItemInInventory());
    }

    public virtual void UseItem(PlayerManager player, ItemInInventory itemUI)
    {

    }

    public virtual void EquipItem()
    {

    }
    public virtual void SpawnInWorld(Vector3 spawnPosition)
    {

    }

    public void Action(PlayerManager player)
    {

    }
}
