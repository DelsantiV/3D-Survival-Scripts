using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class EquippedItemPile : MonoBehaviour
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
            Rigidbody = gameObject.AddComponent<Rigidbody>();
            ShouldDetectCollision = false;
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

        public void StartAction()
        {
            ShouldDetectCollision = true;
        }

        public void EndAction()
        {
            Debug.Log("End of action");
            if (_shouldDetectCollision) ShouldDetectCollision = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collision detected");
            if (!_shouldDetectCollision)
            {
                Debug.Log("Collision ignored");
                return;
            }
            //Vector3 velocity = Rigidbody.linearVelocity;
            ItemPile.OnCollisionDetected(other);
            ShouldDetectCollision = false;
        }

        public void EventTest() { Debug.Log("Wouhou youpi it works"); }
    }
}