using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInInventory : Dragable, IPointerDownHandler
{
    public int amountOfItem;
    protected TextMeshProUGUI amountText;
    protected Canvas canvas;
    protected GameObject itemInfoTemplate;
    protected GameObject itemInfoGO;
    protected GameObject inventoryGO;
    protected bool isOutsideBounds;

    public Item_General_SO ItemSO {get; private set;}
    public IItem Item { get; private set; }

    public PlayerManagerV2 player;
    public InventoryManager inventory;
    public ItemSlot slot;
    public static ItemInInventory activeItem;

    public void CreateItemInInventory(IItem item, int amount)
    {
        this.Item = item;
        ItemSO = item.itemSO;
        Initialize();
        player = PlayerManagerV2.Player;
        inventory = player.GetInventory();
        amountOfItem = amount;
        gameObject.GetComponent<Image>().sprite = ItemSO.iconInInventory;
        amountText = transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
        canvas = FindFirstObjectByType<Canvas>();
        itemInfoTemplate = Resources.Load<GameObject>("UI/ItemInfoTemplate");
        SetAmountOfItem(amountOfItem);
        isOutsideBounds = false;
        slot = transform.parent.GetComponent<ItemSlot>();

        inventoryGO = player.GetInventoryUI().gameObject;
    }

    public void SetAmountOfItem(int amount)
    {
        amountOfItem = amount;
        amountText.text = amountOfItem.ToString();
    }

    public void AddAmountOfItem(int amount)
    {
        SetAmountOfItem(amountOfItem + amount);
    }

    public void RemoveAmountOfItem(int amount)
    {
        SetAmountOfItem(amountOfItem - amount);
    }

    public void MaskItemAmount()
    {
        amountText.gameObject.SetActive(false);
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
            DropItem();
        }
        else { base.OnEndDrag(eventData); }
        if (startParent != transform.parent) // si l'item est déplacé depuis un quickslot vers un autre slot, supprimer la prefab en main du joueur
        {
            RemoveItemFromSlot();
        }
        RefreshSlot();
    }

    public virtual void RemoveItemFromSlot()
    {
        if (slot is QuickSlot)
        {
            QuickSlot quickslot = (QuickSlot) slot;
            quickslot.RemoveItemFromHands();
        }
        slot.RemoveItem();
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
            EquipItemInNextEmptyHand();
        }
    }

    protected virtual void OpenItemInfo()
    {
        if (itemInfoGO == null)
        {
            itemInfoGO = Instantiate(itemInfoTemplate, transform.parent.parent);
            itemInfoGO.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>().SetText(ItemSO.name);

            itemInfoGO.transform.Find("ItemInfoText").GetComponent<TextMeshProUGUI>().SetText(ItemSO.item_info);
            itemInfoGO.transform.Find("DropButton").GetComponent<Button>().onClick.AddListener(DropItem);

            GameObject useButtonGO = itemInfoGO.transform.Find("UseButton").gameObject;
            useButtonGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip";
            useButtonGO.SetActive(true);
            Button useButton = useButtonGO.GetComponent<Button>();
            useButton.onClick.AddListener(EquipItemInNextEmptyHand);
        }
    }

    public virtual void CloseItemInfo()
    {
        if (itemInfoGO != null)
        {
            Destroy(itemInfoGO);
        }
    }

    private void DropItem()
    {
        player.SpawnItemFromPlayer(ItemSO, amountOfItem);
        Destroy(gameObject);
        Debug.Log("Dropped item");
        CloseItemInfo();
    }

    private void UseItem()
    {
        if (Item != null) { Item.UseItem(player); }
        else { }
    }

    public void EquipItemInNextEmptyHand()
    {
        ItemSlot previousSlot = slot;
        if (player.GetHandsManager().TryEquipItemToNextEmptyHand(this))
        {
            Debug.Log("Equiped " + ItemSO.name);
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
            Debug.Log("Equiped " + ItemSO.name);
        }
        else
        {
            Debug.Log("Could not equip item, hand was not empty !");
        }
    }

    public GameObject ItemGO() { return ItemSO.itemPrefab; }
    public string Name { get { return ItemSO.name; } }  
}
