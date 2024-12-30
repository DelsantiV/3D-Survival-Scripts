using UnityEngine;

namespace GoTF.Content
{
    public class BasicTree : BasicResource
    {
        public override void DestroyResource()
        {
            Destroy(gameObject);
        }

        protected override bool CheckToolTierAndType(ItemProperties.DamageProperties damageProperties)
        {
            if (damageProperties.tier >= 1) { return true; }
            return false;
        }
    }
}
