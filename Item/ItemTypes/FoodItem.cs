using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class FoodItem : GeneralItem
    {
        public float caloriesAmount = 1000f;
        public float timeToDigest = 10f;

        ItemProperties.NutritionProperties nutritionProperties;

        public FoodItem()
        {
            JObject jsonParsedFile = JObject.Parse(ItemSO.classProperties);
            JsonConvert.PopulateObject(jsonParsedFile["nutrition_properties"].ToString(), nutritionProperties);
        }

        public override void Initialize(Item_General_SO itemSO)
        {
            base.Initialize(itemSO);

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
