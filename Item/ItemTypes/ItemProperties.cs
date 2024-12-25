using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class ItemProperties : MonoBehaviour
    {
        public struct NutritionProperties
        {
            public float calories;
            public float requiredHydration;
            public float carbohydrates;
            public float sugars;
            public float lipids;
            public float proteins;
            public float vitamins;
            public float minerals;
            public float timeToDigest;
        }
        //public NutritionProperties defaultNutritionProperties;


        public struct EquippableProperties
        {

        }

        public struct ToolProperties
        {

        }

        public struct MeleeWeaponProperties
        {
            public int damageTier;
            public float damageAmount;
            public int animationID;
        }

        public struct ThrowableProperties
        {

        }

        public struct DamageProperties
        {
            public int tier;
            public float amount;
            public DamageSource source;
        }
    }
}
