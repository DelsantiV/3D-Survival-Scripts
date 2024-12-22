using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class FoodItem : GeneralItem
    {
        public ItemProperties.NutritionProperties nutritionProperties;

        public FoodItem()
        {
            
        }

        public override void Initialize(Item_General_SO itemSO)
        {
            base.Initialize(itemSO);
            JObject jsonParsedFile = JObject.Parse(ItemSO.classProperties);
            if (jsonParsedFile != null) {
                if (jsonParsedFile["nutrition_properties"] != null)
                {
                    nutritionProperties = JsonConvert.DeserializeObject<ItemProperties.NutritionProperties>(jsonParsedFile["nutrition_properties"].ToString());
                }
            }
        }

        public override void UseItem(PlayerManager player, ItemPileInInventory pileUI)
        {
            bool isEaten = player.TryEatFood(this);
            if (isEaten)
            {
                pileUI.RemoveItemPileFromSlot();
            }
        }
    }
}
