using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : GeneralItem
{
    public float caloriesAmount = 1000f;
    public float timeToDigest = 10f;

    ItemProperties.NutritionProperties nutritionProperties;

    public override void UseItem(PlayerManagerV2 player, ItemInInventory itemUI)
    {
        base.UseItem(player, itemUI);
        bool isEaten = player.TryEatFood(this);
        if (isEaten)
        {
            itemUI.RemoveAmountOfItem(1);
        }
    }
}
