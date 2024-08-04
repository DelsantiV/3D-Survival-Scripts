using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    public GeneralItem Item { get; private set; }

    public PlayerManager player;
    public InventoryManager inventory;
    public ItemSlot slot;
    public static ItemInInventory activeItem; //replace with true/false bool to avoid use of static

    public void CreateItemInInventory(GeneralItem item, int amount)
    {
        this.Item = item;
        ItemSO = item.ItemSO;
        Initialize();
        inventory = player.GetInventory();
        amountOfItem = amount;
        gameObject.GetComponent<Image>().sprite = ItemSO.iconInInventory;
        amountText = transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
        canvas = FindFirstObjectByType<Canvas>();
        itemInfoTemplate = Resources.Load<GameObject>("UI/ItemInfoTemplate");
        SetAmountOfItem(amountOfItem);
        isOutsideBounds = false;
        slot = transform.parent.GetComponent<ItemSlot>();

        //inventoryGO = player.GetInventoryUI().gameObject;
    }

    public override void Initialize()
    {
        base.Initialize();
        if (Item != null)
        {
            ItemSO = Item.ItemSO;
        }
        player = PlayerManager.Player;
        amountText = transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
        SetAmountOfItem(amountOfItem);
        canvas = FindFirstObjectByType<Canvas>();
        itemInfoTemplate = Resources.Load<GameObject>("UI/ItemInfoTemplate"); // Replace with Addressables ?
        inventoryGO = transform.parent.parent.gameObject;
        isOutsideBounds = false;
        slot = transform.parent.GetComponent<ItemSlot>();
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

    private void DropItem()
    {
        player.SpawnItemFromPlayer(Item, amountOfItem);
        Destroy(gameObject);
        Debug.Log("Dropped item");
        CloseItemInfo();
    }

    private void UseItem()
    {
        if (Item != null) { Item.UseItem(player, this); }
        else { }
    }

    public void EquipItemInNextEmptyHand()
    {
        ItemSlot previousSlot = slot;
        if (player.HandsManager.TryEquipItemToNextEmptyHand(Item))
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
        if (player.HandsManager.TryEquipItemToHand(Item, hand))
        {
            Debug.Log("Equiped " + ItemSO.name);
        }
        else
        {
            Debug.Log("Could not equip item, hand was not empty !");
        }
    }

    public void SetItem(GeneralItem item, int amount = 1) 
    { 
        Item = item; 
        amountOfItem = amount;
        gameObject.GetComponent<Image>().sprite = item.ItemSprite;
    }
}
