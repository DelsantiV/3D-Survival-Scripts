using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public abstract class BasicResource : MonoBehaviour, IDamageable
    {
        [SerializeField] internal float maxLife;
        internal float currentLife;
        [SerializeField] internal AudioClip hitSound;
        protected AudioSource audioSource;

        protected virtual void Awake()
        {
            currentLife = maxLife;
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        public virtual void TakeDamage(ItemProperties.DamageProperties damageProperties)
        {
            if (CheckToolTierAndType(damageProperties))
            {
                Debug.Log(gameObject.name + " was hit !");
                audioSource.PlayOneShot(hitSound);
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
