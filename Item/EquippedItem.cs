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
                return ItemsInWorld != null ? ItemsInWorld.ItemPile : null;
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
        public new Collider collider;
        private PlayerManager Player
        {
            get
            {
                return transform.root.GetComponent<PlayerManager>();
            }
        }
        private Rigidbody Rigidbody;

        private bool _shouldDetectCollision;

        public bool ShouldDetectCollision
        {
            get
            {
                return _shouldDetectCollision;
            }
            set
            {
                _shouldDetectCollision = value;
                Rigidbody.detectCollisions = value;
            }
        }


        private void Awake()
        {
            _shouldDetectCollision = false;
            Rigidbody = gameObject.AddComponent<Rigidbody>();
            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true;
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
            ItemPile.Use(Player, this);
        }
        public void StopUse()
        {
            ItemPile.StopUse(Player, this);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!_shouldDetectCollision) return;
            ItemPile.OnCollisionDetected(other);
        }
    }
}