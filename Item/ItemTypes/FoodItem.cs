using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GoTF.Content.ItemProperties;

namespace GoTF.Content
{
    public class FoodItem : GeneralItem
    {
        public NutritionProperties NutritionProperties {get; private set;}

        public FoodItem()
        {
            
        }

        public override void Initialize(Item_General_SO itemSO)
        {
            base.Initialize(itemSO);
            JObject jsonParsedFile = JObject.Parse(ItemSO.classProperties);
            if (jsonParsedFile != null) {
                if (Utilities.JSONUtilities.DoesJSONContainField(jsonParsedFile,"nutrition_properties"))
                {
                    NutritionProperties = JsonConvert.DeserializeObject<NutritionProperties>(jsonParsedFile["nutrition_properties"].ToString());
                }
            }
        }

        public override void OnItemSpawned()
        {

        }

        public override void OnItemInstanceGenerated()
        {

        }

        public override void UseItem(PlayerManager player, EquippedItemPile item)
        {
            bool isEaten = player.TryEatFood(this);
            if (isEaten)
            {
                item.ItemPile.RemoveItemFromPile(this);
            }
        }

        public override void StopUsingItem(PlayerManager player, EquippedItemPile item)
        {

        }
    }
}
