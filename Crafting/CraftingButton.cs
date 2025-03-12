using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoTF.Content
{
    public class CraftingButton : BasicUI
    {
        public Button button;
        private TextMeshProUGUI buttonText;
        private Crafting.CraftingAction craftingAction;
        private CraftingManager craftingManager;
        [SerializeField] Color disabledTextColor;
        [SerializeField] Color enabledTextColor;

        public void Initialize(CraftingManager craftingManager, Crafting.CraftingAction craftingAction)
        {
            button = GetComponent<Button>();
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
            this.craftingManager = craftingManager;
            this.craftingAction = craftingAction;
            SetButtonCraftingAction(craftingAction);
        }

        private void DisableButton()
        {
            button.interactable = false;
            buttonText.color = disabledTextColor;
        }
        
        private void EnableButton()
        {
            button.interactable = true;
            buttonText.color = enabledTextColor;
        }

        private void SetButtonCraftingAction(Crafting.CraftingAction craftingAction)
        {
            this.craftingAction = craftingAction;
            SetButtonText(craftingAction.ToString());
        }
        private void SetButtonText(string text) { buttonText.text = text; }

        public void UpdateButton()
        {
            button.onClick.RemoveAllListeners();
            List<CraftingRecipe> possibleRecipes = craftingManager.PossibleCraftingRecipesForAction(craftingAction);
            if (possibleRecipes.Count == 0) DisableButton();
            else
            {
                EnableButton();
                button.onClick.AddListener(() => craftingManager.PerformCraftingRecipe(possibleRecipes[0]));
                button.onClick.AddListener(() => UpdateButton());
            }

        }

        public void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}