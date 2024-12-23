using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GoTF.Content.ItemProperties;

namespace GoTF.Content
{
    public class WeaponItem : GeneralItem
    {
        public MeleeWeaponProperties WeaponProperties {get; private set;}
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
                    if (WeaponProperties.animator_path != null) 
                    {
                        //Load AnimatorOverrideController from Addressables (should find a way to wait for it to finish befire launching game ?)
                    }
                }
            }
        }

        public override void UseItem(PlayerManager player, ItemPileInInventory itemPileInInventory)
        {

        }
    }
}
