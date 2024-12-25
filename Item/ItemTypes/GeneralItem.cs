using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public abstract class GeneralItem
    {
        public Item_General_SO ItemSO { get; protected set; }

        public event Action<PlayerManager, GeneralItem> OnItemUsed;

        public GameObject ItemPrefab { get { return ItemSO.itemPrefab; } }
        public Sprite ItemSprite { get { return ItemSO.iconInInventory; } }
        public string ItemName { get { return ItemSO.name; } }

        /// <summary>
        /// Item weight in kg
        /// </summary>
        public float Weight { get { return ItemSO.weight; } }

        /// <summary>
        /// 1/Bulk = number of items a normal human can carry
        /// </summary>
        public float Bulk { get { return ItemSO.bulk; } }

        public int animationID;

        public GeneralItem()
        {

        }

        public virtual void Initialize(Item_General_SO itemSO)
        {
            ItemSO = itemSO;
        }

        public abstract void UseItem(PlayerManager player, ItemPileInInventory pileUI);
    }
}
