using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public abstract class BasicResource : MonoBehaviour, IDamageable
    {
        [SerializeField] internal float maxLife;
        internal float currentLife;

        public virtual void TakeDamage(ItemProperties.DamageProperties damageProperties)
        {
            if (CheckToolTierAndType(damageProperties))
            {
                Debug.Log(gameObject.name + " was hit !");
                currentLife -= damageProperties.amount;
                if (currentLife < 0)
                {
                    DestroyResource();
                }
            }
        }
        protected abstract bool CheckToolTierAndType(ItemProperties.DamageProperties damageProperties); //Should depend on the item used
        public abstract void DestroyResource();
    }
}
