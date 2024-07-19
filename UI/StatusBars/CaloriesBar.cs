using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaloriesBar : StatusBar
{
    public override void Awake()
    {
        base.Awake();

        CustomTickSystem.OnLargeTick += UpdateValues;
    }
    public override void Start()
    {
        base.Start();
        UpdateValues();
        UpdateSlider();
    }

    private void UpdateValues()
    {
        maxAmount = status.maxCalories;
        currentAmount = status.currentCalories;
    }
}
