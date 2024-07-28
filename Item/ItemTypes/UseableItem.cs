using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UseableItem : ItemInInventory
{
    public static event Action<PlayerManager, UseableItem> OnItemUsed;
    protected override void OpenItemInfo()
    {
        base.OpenItemInfo();
        GameObject useButtonGO = itemInfoGO.transform.Find("UseButton").gameObject;
        useButtonGO.SetActive(true);
        Button useButton = useButtonGO.GetComponent<Button>();
        useButton.onClick.AddListener(UseItem);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
    }

    public virtual void UseItem()
    {
        Debug.Log("Used "+itemSO.name);
        OnItemUsed?.Invoke(player, this);
        // player defined in ItemInInventory script
    }
}
