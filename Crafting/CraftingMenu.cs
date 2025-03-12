using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GoTF.Content
{
    public class CraftingMenuUI : BasicUI
    {
        private PlayerManager player;
        public CraftingManager CraftingManager { get { return player.CraftingManager; } }
        private float craftButtonOffset = 5f;
        private Dictionary<Crafting.CraftingAction,CraftingButton> craftingButtonsDict;
        public void Initialize(PlayerManager player)
        {
            SetPlayer(player);
            craftingButtonsDict = new();
            CraftingButton templateButton = transform.Find("TemplateButton").GetComponent<CraftingButton>();
            Vector2 currentButtonPosition = templateButton.Position;
            Vector2 buttonOffset = new Vector2(0, templateButton.Size.y + craftButtonOffset);
            foreach (Crafting.CraftingAction craft in Enum.GetValues(typeof(Crafting.CraftingAction)))
            {
                CraftingButton craftButton = Instantiate(templateButton, transform);
                craftButton.SetPosition(currentButtonPosition);
                craftingButtonsDict.Add(craft, craftButton);
                currentButtonPosition -= buttonOffset;
                craftButton.Initialize(CraftingManager, craft);
            }
            Destroy(templateButton);
        }

        private void SetPlayer(PlayerManager player)
        {
            this.player = player;
        }
        public void UpdateCraftingRecipes()
        {
            foreach(CraftingButton craftButton in craftingButtonsDict.Values)
            {
                craftButton.UpdateButton();
            }
        }

        public override void OpenUI()
        {
            UpdateCraftingRecipes();
            base.OpenUI();
        }
    }
}
