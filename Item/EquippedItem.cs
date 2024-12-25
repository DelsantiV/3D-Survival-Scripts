using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class EquippedItem : MonoBehaviour
    {
        public ItemPileInWorld ItemsInWorld
        {
            get
            {
                return GetComponent<ItemPileInWorld>();
            }
        }
        public ItemPile ItemPile
        {
            get
            {
                return ItemsInWorld.ItemPile;
            }
        }
        public int AnimationID
        {
            get 
            {
                if (ItemPile == null) return 0;
                else return ItemPile.AnimationID; 
            }
        }

        private void Awake()
        {
        }
        public void Remove()
        {
            Destroy(gameObject);
        }

        public void Drop()
        {
            transform.parent = null;
            ItemsInWorld.AddRigidBodyToItems();
        }

        public void ChangeParent(Transform transform, bool worldPositionStays = false)
        {
            if (transform.parent == null)
            {
                Drop();
                return;
            }

            transform.SetParent(transform, worldPositionStays);
        }

        public void Use()
        {

        }
    }
}