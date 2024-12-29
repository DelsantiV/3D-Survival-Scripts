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
        /// <summary>
        /// The prefab of this item
        /// </summary>
        public GameObject ItemPrefab { get { return ItemSO.itemPrefab; } }
        /// <summary>
        /// The sprite for the item's icon
        /// </summary>
        public Sprite ItemSprite { get { return ItemSO.iconInInventory; } }
        /// <summary>
        /// The name of this item
        /// </summary>
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
        /// <summary>
        /// Must be called when creating the item
        /// </summary>
        /// <param name="itemSO"></param>
        public virtual void Initialize(Item_General_SO itemSO)
        {
            ItemSO = itemSO;
        }

        /// <summary>
        /// What happens when the item is used by the player
        /// </summary>
        /// <param name="player"The player using the item></param>
        /// <param name="item"The EquippedItem from which the item is used></param>
        public abstract void UseItem(PlayerManager player, EquippedItem item);
        /// <summary>
        /// What happens when the player stops using the item
        /// </summary>
        /// <param name="player"></param>
        /// <param name="item"></param>
        public abstract void StopUsingItem(PlayerManager player, EquippedItem item);
        /// <summary>
        /// Called when the EquippedItem detects a collision
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnCollisionDetected(Collider other) { }
    }
}
