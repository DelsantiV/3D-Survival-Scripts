using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : StatusBar
{
    public override void UpdateValues()
    {
        maxAmount = status.maxHealth;
        currentAmount = status.currentHealth;
    }
}
