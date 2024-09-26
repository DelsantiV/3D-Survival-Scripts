using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemPileInInventory : Dragable, IPointerDownHandler
{
    public int amountOfItem;
    protected TextMeshProUGUI amountText;
    protected Canvas canvas;
    //protected GameObject itemInfoTemplate;
    protected GameObject itemInfoGO;
    protected GameObject inventoryGO;
    protected bool isOutsideBounds;

    public ItemPile ItemPile { get; private set; }

    public PlayerManager player;
    public ItemSlot slot;
    public static ItemPileInInventory activeItem;

    public void Initialize(PlayerManager player)
    {
        base.Initialize();
        this.player = player;
        canvas = FindFirstObjectByType<Canvas>();
        //itemInfoTemplate = Resources.Load<GameObject>("UI/ItemInfoTemplate"); // Replace with Addressables ?
        inventoryGO = transform.parent.gameObject;
        isOutsideBounds = false;
        RefreshSlot();
    }

    public void SetItemPile(ItemPile itemPile, PlayerManager player)
    {
        ItemPile = itemPile;
        gameObject.GetComponent<Image>().sprite = ItemManager.PileIcon;
        Initialize(player);
    }

    public void RefreshSlot()
    {
        slot = transform.parent.GetComponent<ItemSlot>();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        CloseItemInfo();
    }
    public override void OnDrag(PointerEventData eventData)
    {
        //Make movement consistent with canvas scale
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;

        // Need to modify to correctly get bounds
        if (rectTransform.anchoredPosition.x > inventoryGO.GetComponent<RectTransform>().sizeDelta.x / 2 + inventoryGO.GetComponent<RectTransform>().anchoredPosition.x)
        {
            gameObject.GetComponent<Image>().color = Color.red;
            isOutsideBounds = true; 
        }  
        else { 
            gameObject.GetComponent<Image>().color = Color.white; 
            isOutsideBounds = false;
        }

    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //Drop item if outside inventory bounds
        if (isOutsideBounds)
        {
            DropPile();
        }
        else { base.OnEndDrag(eventData); }
    }

    public virtual void RemoveItemPileFromSlot()
    {
        if (slot is QuickSlot) // si l'item est d�plac� depuis un quickslot, supprimer la prefab en main du joueur
        {
            QuickSlot quickslot = (QuickSlot) slot;
            quickslot.RemoveItemFromHands();
        }
        slot.RemovePile();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (activeItem != this) 
        {
            if (activeItem != null) { activeItem.CloseItemInfo(); }
            activeItem = this;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OpenItemInfo();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            EquipPileInNextEmptyHand();
        }
    }

    protected virtual void OpenItemInfo()
    {
        if (itemInfoGO == null && slot is not QuickSlot)
        {
            /*
            itemInfoGO = Instantiate(itemInfoTemplate, canvas.transform);
            itemInfoGO.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>().SetText(ItemSO.name);

            itemInfoGO.transform.Find("ItemInfoText").GetComponent<TextMeshProUGUI>().SetText(ItemSO.item_info);
            itemInfoGO.transform.Find("DropButton").GetComponent<Button>().onClick.AddListener(DropItem);

            GameObject useButtonGO = itemInfoGO.transform.Find("UseButton").gameObject;
            useButtonGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip";
            useButtonGO.SetActive(true);
            Button useButton = useButtonGO.GetComponent<Button>();
            useButton.onClick.AddListener(EquipItemInNextEmptyHand);
            */
            Debug.Log("Should show Info");
        }
    }

    public virtual void CloseItemInfo()
    {
        if (itemInfoGO != null)
        {
            Destroy(itemInfoGO);
        }
    }

    private void DropPile()
    {
        player.SpawnPileFromPlayer(ItemPile);
        Destroy(gameObject);
        Debug.Log("Dropped item");
        CloseItemInfo();
    }

    private void UsePile()
    {
        if (ItemPile.IsPileUniqueItem) { ItemPile.FirstItemInPile.Action(player); }
        else { }
    }

    public void EquipPileInNextEmptyHand()
    {
        ItemSlot previousSlot = slot;
        if (player.HandsManager.TryEquipItemPileToNextEmptyHand(ItemPile))
        {
            Debug.Log("Equiped " + ItemPile.ToString());
            previousSlot.RemovePile();
            RefreshSlot();
        }
        else
        {
            Debug.Log("Could not equip item, no empty hand found !");
        }
    }

    public void EquipItemInHand(HandsManager.Hand hand)
    {
        if (player.HandsManager.TryEquipItemPileToHand(ItemPile, hand))
        {
            Debug.Log("Equiped " + ItemPile.ToString());
        }
        else
        {
            Debug.Log("Could not equip item, hand was not empty !");
        }
    }
}