using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GoTF.Content.ItemProperties;

namespace GoTF.Content
{
    public abstract class BasicResource : MonoBehaviour, IDamageable
    {
        [SerializeField] internal float maxLife;
        internal float currentLife;
        [SerializeField] internal AudioClip hitSound;
        protected AudioSource audioSource;
        protected ParticleSystem hitParticles;

        protected virtual void Awake()
        {
            currentLife = maxLife;
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            TryGetComponent<ParticleSystem>(out hitParticles);            
        }
        public virtual void TakeDamage(DamageProperties damageProperties)
        {
            if (CheckToolTierAndType(damageProperties))
            {
                TakeHit(damageProperties);
            }
        }

        protected virtual void TakeHit(DamageProperties damageProperties)
        {
            Debug.Log(gameObject.name + " was hit !");
            if (audioSource != null)  audioSource.PlayOneShot(hitSound);
            if (hitParticles != null) hitParticles.Play();
            currentLife -= damageProperties.amount;
            if (currentLife < 0)
            {
                DestroyResource();
            }
        }
        protected abstract bool CheckToolTierAndType(DamageProperties damageProperties); //Should depend on the item used
        public abstract void DestroyResource();
    }
}
