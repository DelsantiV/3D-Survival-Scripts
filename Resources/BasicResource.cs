using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class BasicResource : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxLife;
        private float currentLife;

        // Update is called once per frame
        public void TakeDamage(float damage, DamageSource damageSource)
        {
            Debug.Log(gameObject.name + " was hit !");
            currentLife -= damage;
            if (currentLife < 0)
            {
                DestroyResource();
            }
        }

        public void DestroyResource()
        {

        }
    }
}
