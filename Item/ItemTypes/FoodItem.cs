using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : UseableItem
{
    public float caloriesAmount = 1000f;
    public float timeToDigest = 10f;

    ItemProperties.NutritionProperties nutritionProperties;

    public override void UseItem()
    {
        base.UseItem();
        bool isEaten = player.TryEatFood(this);
        if (isEaten)
        {
            inventory.RemoveItemFromInventory(slot, 1);
        }
    }
}
