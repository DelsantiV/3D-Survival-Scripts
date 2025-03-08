using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoTF.Utilities;

namespace GoTF.Content
{
    public class DigestiveSystem
    {
        public List<FoodItem> containedFood;
        public FoodItem foodBeingDigested;
        private float currentDigestionProgress;
        private PlayerStatus playerStatus;


        public void InitializeDigestiveSystem(PlayerStatus playerStatus)
        {
            CustomTickSystem.OnLargeTick += DigestFood;
            this.playerStatus = playerStatus;
            containedFood = new List<FoodItem>();
        }

        public DigestiveSystem(PlayerStatus playerStatus, List<FoodItem> containedFood)
        {
            InitializeDigestiveSystem(playerStatus);
            this.containedFood = containedFood;
        }

        public DigestiveSystem(PlayerStatus playerStatus)
        {
            InitializeDigestiveSystem(playerStatus);
        }


        public void AddFoodToDigestiveSystem(FoodItem foodItem)
        {
            containedFood.Add(foodItem);
            // Maybe try to add at the end of liste so digestion is simpler
        }

        public bool TryAddFoodToDigestiveSystem(FoodItem foodItem)
        {
            if (IsFull()) { return false; }
            else
            {
                AddFoodToDigestiveSystem(foodItem);
                return true;
            }
        }

        public bool IsFull()
        {
            return containedFood.Count > 5;
        }

        public bool IsDigesting()
        {
            return foodBeingDigested != null;
        }

        public void DigestFood()
        {
            if (foodBeingDigested == null)
            {
                if (containedFood.Count > 0)
                {
                    foodBeingDigested = containedFood[containedFood.Count - 1];
                    playerStatus.isDigesting = true;
                }
                else
                {
                    return;
                }
            }

            playerStatus.currentCalories += foodBeingDigested.NutritionProperties.calories / foodBeingDigested.NutritionProperties.timeToDigest;

            currentDigestionProgress += 1 / foodBeingDigested.NutritionProperties.timeToDigest;
            if (currentDigestionProgress >= 1)
            {
                containedFood.RemoveAt(containedFood.Count - 1);
                if (containedFood.Count > 0)
                {
                    foodBeingDigested = containedFood[containedFood.Count - 1];
                }
                else
                {
                    foodBeingDigested = null;
                    playerStatus.isDigesting = false;
                }
                currentDigestionProgress = 0;
            }
        }
    }
}
