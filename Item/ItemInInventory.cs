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

    public Item_General_SO itemSO;

    public PlayerManager player;
    public InventoryManager inventory;
    public ItemSlot slot;
    public static ItemInInventory activeItem;

    public void CreateItemInInventory(Item_General_SO itemSO, int amount)
    {
        Initialize();
        player = PlayerManager.Player;
        inventory = player.GetInventory();
        this.itemSO = itemSO; 
        amountOfItem = amount;
        gameObject.GetComponent<Image>().sprite = itemSO.iconInInventory;
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
    }

    protected virtual void OpenItemInfo()
    {
        if (itemInfoGO == null)
        {
            itemInfoGO = Instantiate(itemInfoTemplate, transform.parent.parent);
            itemInfoGO.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>().SetText(itemSO.name);

            itemInfoGO.transform.Find("ItemInfoText").GetComponent<TextMeshProUGUI>().SetText(itemSO.item_info);
            itemInfoGO.transform.Find("DropButton").GetComponent<Button>().onClick.AddListener(DropItem);
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
        player.SpawnItemFromPlayer(itemSO, amountOfItem);
        Destroy(gameObject);
        Debug.Log("Dropped item");
        CloseItemInfo();
    }

    public GameObject ItemGO() { return itemSO.itemPrefab; }
    public string Name { get { return itemSO.name; } }  
}
