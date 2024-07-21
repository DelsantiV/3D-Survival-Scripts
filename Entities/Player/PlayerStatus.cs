using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
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

    public PlayerStatus(float maxHealth, float maxFatigue, float maxCalories)
    {
        this.maxHealth = maxHealth;
        this.maxFatigue = maxFatigue;
        this.maxCalories = maxCalories;

        CustomTickSystem.OnLargeTick += UpdatePlayerStatus;

        // Initialize to some low values for testing
        currentHealth = 1;
        currentCalories = 1000;
    }

    public void UpdatePlayerStatus()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = (float) Math.Round(Math.Min(maxHealth, currentHealth + healthRegenPerSecond), 1);
        }

        if (currentCalories > 0 && !isDigesting)
        {
            currentCalories = (float)Math.Round(Math.Max(0, currentCalories - caloriesDecayPerSecond), 1);
        }
    }
}
