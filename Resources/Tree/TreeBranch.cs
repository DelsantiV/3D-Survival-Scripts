using UnityEngine;

namespace GoTF.Content
{
    public class TreeBranch : BasicResource
    {
        private float maxSpeedMagnitude = 0.5f;
        private TreeDebris treeDebris;
        private Collider boxCollider;

        private bool IsBroken
        {
            get
            {
                return transform.parent == null;
            }
        }

        protected override void Awake()
        {
            boxCollider = transform.Find("Collider").GetComponent<Collider>();
            boxCollider.enabled = false;
        }

        public override void DestroyResource()
        {
            transform.parent = null;
            treeDebris.BreakBranch();
            Debug.Log("Branch broke !");
        }

        protected override bool CheckToolTierAndType(ItemProperties.DamageProperties damageProperties)
        {
            if (damageProperties.tier >= 1) { return true; }
            return false;
        } 
        private void OnEnable()
        {
            treeDebris = transform.parent.GetComponent<TreeDebris>();
            boxCollider.enabled = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Branch Collided");
            if (collision.relativeVelocity.magnitude > maxSpeedMagnitude && IsBroken) 
            {
                DestroyResource();
            }
        }
    }
}