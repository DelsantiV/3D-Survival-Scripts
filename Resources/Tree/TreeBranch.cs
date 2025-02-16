using UnityEngine;

namespace GoTF.Content
{
    public class TreeBranch : BasicResource
    {
        private float maxSpeedMagnitude = 0.5f;
        private TreeDebris treeDebris;
        private Rigidbody rb;
        private FixedJoint joint;  

        private bool IsBroken
        {
            get
            {
                return transform.parent == null;
            }
        }

        protected override void Awake()
        {
            rb = GetComponent<Rigidbody>();
            joint = GetComponent<FixedJoint>();
        }

        public override void DestroyResource()
        {
            Destroy(joint);
            rb.automaticCenterOfMass = true;
            transform.parent = null;
            treeDebris.BreakBranch();
            ItemManager.TurnObjectIntoItem(gameObject, "stick");
            Debug.Log("Branch broke !");
            Destroy(this);
        }

        protected override bool CheckToolTierAndType(ItemProperties.DamageProperties damageProperties)
        {
            if (damageProperties.tier >= 1) { return true; }
            return false;
        } 
        private void OnEnable()
        {
            treeDebris = transform.parent.GetComponent<TreeDebris>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Branch Collided");
            if (collision.relativeVelocity.magnitude > maxSpeedMagnitude && !IsBroken) 
            {
                DestroyResource();
            }
        }
    }
}