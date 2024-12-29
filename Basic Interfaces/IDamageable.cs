using System.Collections;
using static GoTF.Content.ItemProperties;
using UnityEngine;

namespace GoTF.Content
{
    public interface IDamageable
    {
        public void TakeDamage(DamageProperties damageProperties);
    }

    public enum DamageSource
    {
        PlayerHit,
        Explosion,
        Ennemy
    }
}
