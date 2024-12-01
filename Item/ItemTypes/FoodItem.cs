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

        public FoodItem(Item_General_SO itemSO) : base(itemSO)
        {

        }

        public override void UseItem(PlayerManager player, ItemPileInInventory pileUI)
        {
            base.UseItem(player, pileUI);
            bool isEaten = player.TryEatFood(this);
            if (isEaten)
            {
                pileUI.RemoveItemPileFromSlot();
            }
        }
    }
}
