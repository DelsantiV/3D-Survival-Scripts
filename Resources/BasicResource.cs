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
        protected ParticleSystem hitParticles;

        public virtual void Awake()
        {
            currentLife = maxLife;
            audioSource = gameObject.AddComponent<AudioSource>();
            hitParticles = GetComponent<ParticleSystem>();
        }
        public virtual void TakeDamage(ItemProperties.DamageProperties damageProperties)
        {
            if (CheckToolTierAndType(damageProperties))
            {
                Debug.Log(gameObject.name + " was hit !");
                audioSource.PlayOneShot(hitSound);
                hitParticles.Play();
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
