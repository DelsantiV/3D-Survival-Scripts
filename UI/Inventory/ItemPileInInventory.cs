using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoTF.Content
{
    public class ItemPileInInventory : Dragable, IPointerDownHandler
    {
        protected Canvas canvas;
        protected CanvasManager canvasManager;
        protected PileMenu pileMenu;
        //protected GameObject itemInfoTemplate;
        protected GameObject itemInfoGO;
        protected GameObject inventoryGO;
        protected bool isOutsideBounds;
        protected int numberOfSlotsLeftOnIcon;

        public ItemPile ItemPile { get; private set; }

        public PlayerManager player;
        public ItemSlot slot;
        public static ItemPileInInventory activeItem;

        public void Initialize(PlayerManager player)
        {
            base.Initialize();
            this.player = player;
            canvas = FindFirstObjectByType<Canvas>();
            canvasManager = canvas.GetComponent<CanvasManager>();
            //itemInfoTemplate = Resources.Load<GameObject>("UI/ItemInfoTemplate"); // Replace with Addressables ?
            inventoryGO = transform.parent.gameObject;
            isOutsideBounds = false;
            pileMenu = null;
            RefreshSlot();
        }

        public void SetItemPile(ItemPile itemPile, PlayerManager player)
        {
            ItemPile = itemPile;
            itemPile.OnPileChanged.AddListener(UpdatePileIcon);
            if (itemPile.NumberOfItemsInPile > 0) { CreatePileIcon(); }
            Initialize(player);
        }

        public void RefreshSlot()
        {
            slot = transform.parent.GetComponent<ItemSlot>();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            ClosePileInfo();
        }
        public override void OnDrag(PointerEventData eventData)
        {
            //Make movement consistent with canvas scale
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

            // Need to modify to correctly get bounds
            if (rectTransform.anchoredPosition.x > inventoryGO.GetComponent<RectTransform>().sizeDelta.x / 2 + inventoryGO.GetComponent<RectTransform>().anchoredPosition.x)
            {
                gameObject.GetComponent<Image>().color = Color.red;
                isOutsideBounds = true;
            }
            else
            {
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
            slot.RemovePile();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (activeItem != this)
            {
                if (activeItem != null) { activeItem.ClosePileInfo(); }
                activeItem = this;
            }
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OpenPileInfo();
            }
        }

        protected virtual void OpenPileInfo()
        {
            if (pileMenu == null) { pileMenu = canvasManager.OpenPileMenu(this, rectTransform.anchoredPosition); }
            pileMenu.OpenUI();
        }

        public virtual void ClosePileInfo()
        {
            if (pileMenu != null)
            {
                if (pileMenu.IsOpen()) { pileMenu.CloseUI(); }
            }
        }

        private void CreatePileIcon()
        {
            if (ItemPile.NumberOfItemsInPile == 0) {  return; }
            GameObject imageTemplateGO = transform.Find("TemplateImage").gameObject;
            float totalImageSize = imageTemplateGO.GetComponent<RectTransform>().sizeDelta.x;

            int numberOfRows = Mathf.CeilToInt(Mathf.Sqrt(ItemPile.NumberOfItemsInPile));
            int numberOfColumns = Mathf.CeilToInt((float)ItemPile.NumberOfItemsInPile / numberOfRows);
            Debug.Log("Creating pile icon with " + numberOfRows + " rows and " + numberOfColumns + " columns");
            imageTemplateGO.GetComponent<RectTransform>().localScale = new Vector3(1.0f / numberOfRows, 1.0f / numberOfRows); // numberOfRows >= numberOfColumns (always)
            float delta = totalImageSize / numberOfRows;
            float startPos = -delta / 2.0f * (numberOfRows - 1);
            Vector2 firstImagePosition = new Vector2(startPos, startPos);

            int i = 0;
            int j = 0;
            foreach (GeneralItem item in ItemPile.ItemsInPile)
            {
                Debug.Log(new Vector2(i, j));
                GameObject itemImage = Instantiate(imageTemplateGO, transform);
                itemImage.SetActive(true);
                itemImage.GetComponent<Image>().sprite = item.ItemSprite;
                itemImage.GetComponent<RectTransform>().anchoredPosition = firstImagePosition + new Vector2(i * delta, j * delta);

                j++;
                j %= numberOfRows;
                if (j == 0)
                {
                    i++;
                }
            }
            numberOfSlotsLeftOnIcon = numberOfColumns - i;
            imageTemplateGO.SetActive(false);
        }

        private void ClearIcon()
        {
            for (int childIndex = transform.childCount - 1; childIndex > 0; childIndex--) { Destroy(transform.GetChild(childIndex).gameObject); }
        }

        /// <summary>
        /// Clears the icon and recreate one based on the ItemPile linked to this ItemPileInInventory.
        /// Better to use AddItemsToIcon when possible.
        /// </summary>
        public void UpdatePileIcon()
        {
            if (ItemPile.IsEmpty) Destroy(gameObject);
            ClearIcon();
            CreatePileIcon();
        }

        /// <summary>
        /// Add new items to the pile icon
        /// </summary>
        /// <param name="itemsToAdd"></param>
        public void AddItemsToIcon(ItemPile itemsToAdd)
        {
            if (itemsToAdd != null)
            {
                if (itemsToAdd.NumberOfItemsInPile <= numberOfSlotsLeftOnIcon)
                {
                    UpdatePileIcon();
                }
                else
                {
                    UpdatePileIcon();
                }
            }
        }

        public void DropPile()
        {
            if (slot is QuickSlot) // For now, all available slots are quickslots
            {
                player.HandsInventory.RemovePileFromHand((slot as QuickSlot).hand, true);
            }
            else
            {

            }
            Destroy(gameObject);
            Debug.Log("Dropped item");
            ClosePileInfo();
        }
    }
}
