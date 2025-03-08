using Unity.VisualScripting;
using UnityEngine;

namespace GoTF.Content
{
    public class CraftingMenuUI : BasicUI
    {
        public CraftingManager craftingManager;


        public void UpdateCraftingRecipes()
        {

        }

        public override void OpenUI()
        {
            UpdateCraftingRecipes();
            base.OpenUI();
        }
    }
}
