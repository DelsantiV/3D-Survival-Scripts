using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippableItem : ItemInInventory
{

    protected override void OpenItemInfo()
    {
        base.OpenItemInfo();
        GameObject useButtonGO = itemInfoGO.transform.Find("UseButton").gameObject;
        useButtonGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip";
        useButtonGO.SetActive(true);
        Button useButton = useButtonGO.GetComponent<Button>();
        useButton.onClick.AddListener(EquipItemInNextEmptyHand);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            EquipItemInNextEmptyHand();
        }
    }

    public override void RemoveItemFromSlot()
    {
        if (slot is QuickSlot)
        {
            QuickSlot quickslot = (QuickSlot) slot; 
            quickslot.RemoveItemFromHands();
        }
        base.RemoveItemFromSlot();
    }

    public void EquipItemInNextEmptyHand()
    {
        ItemSlot previousSlot = slot;
        if (player.GetHandsManager().TryEquipItemToNextEmptyHand(this))
        {
            Debug.Log("Equiped " + itemSO.name);
            previousSlot.RemoveItem();
            RefreshSlot();
        }
        else
        {
            Debug.Log("Could not equip item, no empty hand found !");
        }
    }

    public void EquipItemInHand(HandsManager.Hand hand)
    {
        if (player.GetHandsManager().TryEquipItemToHand(this, hand))
        {
            Debug.Log("Equiped " + itemSO.name);
        }
        else
        {
            Debug.Log("Could not equip item, hand was not empty !");
        }
    }
}
