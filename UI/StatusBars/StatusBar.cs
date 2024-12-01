using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GoTF.Content
{
    public class StatusBar : MonoBehaviour
    {
        private Slider slider;
        private CanvasManager canvasManager;
        private PlayerManager player;
        protected TextMeshProUGUI amountText;
        protected float maxAmount;
        protected float currentAmount;
        protected PlayerStatus status;


        public virtual void Awake()
        {

        }

        public virtual void InitializeStatusBar(PlayerManager player)
        {
            maxAmount = 0;
            currentAmount = 0;
            amountText = transform.Find("FillArea").transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
            slider = GetComponent<Slider>();
            canvasManager = transform.root.GetComponent<CanvasManager>();

            status = player.PlayerStatus;
            UpdateStatusBar();
            CustomTickSystem.OnLargeTick += UpdateStatusBar;
        }

        public virtual void SetAmount(float amount)
        {
            currentAmount = amount;
            if (currentAmount < 0) { currentAmount = 0; }
            UpdateSlider();
        }

        public virtual void AddAmount(float amount)
        {
            SetAmount(currentAmount + amount);
        }


        public virtual void RemoveAmount(float amount)
        {
            SetAmount(currentAmount - amount);
        }

        public virtual void UpdateValues() { }

        public virtual void SetText() { amountText.SetText(currentAmount.ToString() + "/" + maxAmount.ToString()); }
        public virtual void SetText(string text) { amountText.SetText(text); }
        public virtual void SetSlider() { slider.value = currentAmount / maxAmount; }

        public void UpdateSlider()
        {
            SetText();
            SetSlider();
        }

        public virtual void UpdateStatusBar()
        {
            UpdateValues();
            UpdateSlider();
        }
    }
}
