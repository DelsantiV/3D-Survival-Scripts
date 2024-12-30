using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GoTF.Content.ItemProperties;

namespace GoTF.Content
{
    public class WeaponItem : GeneralItem
    {
        public MeleeWeaponProperties WeaponProperties {get; private set;}
        private DamageProperties damageProperties;
        public AnimatorOverrideController AnimatorOverrideController {get; private set;}
        public WeaponItem()
        {

        }

        public override void Initialize(Item_General_SO itemSO)
        {
            base.Initialize(itemSO);

            JObject jsonParsedFile = JObject.Parse(ItemSO.classProperties);
            if (jsonParsedFile != null)
            {
                if (jsonParsedFile["weapon_properties"] != null)
                {
                    WeaponProperties = JsonConvert.DeserializeObject<MeleeWeaponProperties>(jsonParsedFile["weapon_properties"].ToString());
                    animationID = WeaponProperties.animationID;
                }
                damageProperties.amount = WeaponProperties.damageAmount;
                damageProperties.tier = WeaponProperties.damageTier;
                damageProperties.source = DamageSource.PlayerHit;
            }
        }

        public override void UseItem(PlayerManager player, EquippedItem item)
        {
            //item.ShouldDetectCollision = true;
            Debug.Log(ItemName + " is being used");
        }

        public override void StopUsingItem(PlayerManager player, EquippedItem item)
        {
            //item.ShouldDetectCollision = false;
            Debug.Log(ItemName + " stopped being used");
        }

        public override void OnCollisionDetected(Collider other)
        {
            if (other.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damageProperties);
            }
        }
    }
}
