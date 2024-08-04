using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaloriesBar : StatusBar
{
    public override void UpdateValues()
    {
        maxAmount = status.maxCalories;
        currentAmount = status.currentCalories;
    }
}
