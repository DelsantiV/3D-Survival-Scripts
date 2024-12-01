using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public interface IDamageable
    {
        public void TakeDamage(float damageAmount, DamageSource damageSource);
    }

    public enum DamageSource
    {
        PlayerHit,
        Explosion,
        Ennemy
    }
}
