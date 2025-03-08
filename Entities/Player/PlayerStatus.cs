using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class PlayerStatus
    {
        private readonly PlayerManager player;

        public float maxHealth;
        public float currentHealth;
        public float healthRegenPerSecond = 0.2f;

        public float maxFatigue;
        public float currentFatigue;
        public bool isWalking;
        public bool isRunning;

        public float maxCalories;
        public float currentCalories;
        public float caloriesDecayPerSecond = 1f;
        public bool isDigesting = false;

        public float maxCarriyngWeightBothHands = 50f;
        public float maxCarriyngWeightPrefHand = 20f;
        public float maxCarriyngWeightOtherHand = 20f;

        public float maxCarriyngBulkBothHands = 3f;
        public float maxCarriyngBulkPrefHand = 1f;
        public float maxCarriyngBulkOtherHand = 1f;

        public float CurrentCarriedWeight
        {
            get
            {
                return player.HandsInventory.CurrentTotalCarryingWeight;
            }
        }

        public bool CanSprint
        {
            get
            {
                return CurrentCarriedWeight < 5;
            }
        }

        public PlayerStatus(PlayerManager player, float maxHealth, float maxFatigue, float maxCalories)
        {
            this.player = player;
            this.maxHealth = maxHealth;
            this.maxFatigue = maxFatigue;
            this.maxCalories = maxCalories;

            Utilities.CustomTickSystem.OnLargeTick += UpdatePlayerStatus;

            // Initialize to some low values for testing
            currentHealth = 1;
            currentCalories = 1000;
        }

        public void UpdatePlayerStatus()
        {
            if (currentHealth < maxHealth)
            {
                currentHealth = (float)Math.Round(Math.Min(maxHealth, currentHealth + healthRegenPerSecond), 1);
            }

            if (currentCalories > 0 && !isDigesting)
            {
                currentCalories = (float)Math.Round(Math.Max(0, currentCalories - caloriesDecayPerSecond), 1);
            }
        }
    }
}
