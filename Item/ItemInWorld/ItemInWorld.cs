using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace GoTF.Content
{
    public class ItemInWorld : MonoBehaviour, IInteractable
    {
        public GeneralItem item;
        public ItemPileInWorld itemPileInWorld
        {
            get
            {
                return transform.root.GetComponent<ItemPileInWorld>();
            }
        }

        private Rigidbody rb;
        public virtual void PickUpItem(PlayerManager player)
        {
            if (item != null && player.HandsInventory != null)
            {
                if (player.TryCollectItem(this))
                {
                    //if (itemPileInWorld != null) { itemPileInWorld.RemoveItem(item); }
                    //else { Destroy(gameObject); }
                    if (rb != null) Destroy(rb);
                    Debug.Log("Picked up " + ItemName + "!");
                }
                else
                {
                    Debug.Log("Cannot collect " + ItemName + " !");
                }
            }
            else { Debug.Log("Problemos"); }
        }

        public GameObject ItemPrefab { 

            get {
                if (item.ItemPrefab != null) return item.ItemPrefab;
                else return gameObject;
            } 
        }
        public Sprite ItemSprite { get { return item.ItemSprite; } }
        public string ItemName { get { return item.ItemName; } }
        public string ObjectName { get { return ItemName + " [E to pick up]"; } }
        public float Weight { get { return item.Weight; } }
        public float Bulk { get { return item.Bulk; } }
        public bool ShouldDisplayNameOnMouseOver
        {
            get
            {
                return true;
            }
        }

        protected virtual void Start()
        {
            gameObject.TryGetComponent<Rigidbody>(out rb);
            item.OnItemSpawned();
        }

        protected virtual void Update()
        {
            if (transform.position.y < -50) { Destroy(gameObject); }
        }

        public virtual void OnCollectInteract(PlayerManager player)
        {
            PickUpItem(player);
        }

        public virtual void RemoveFromPile()
        {
            if (itemPileInWorld != null) { itemPileInWorld.RemoveItem(this); }
        }
    }
}