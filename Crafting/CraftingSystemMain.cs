using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class CraftingManager
    {
        private readonly PlayerManager player;
        private readonly HandsInventory handsInventory;
        public CraftingManager(PlayerManager player)
        {
            this.player = player;
            this.handsInventory = player.HandsInventory;
        }



        public void UpdateCraftingRecipes()
        {

        }
    }

    public class CraftingRecipes
    {
        public static List<CraftingRecipe> allCraftingRecipes { get; set; }
        public static List<CraftingProcess> allCraftingProcesses { get; set; }
    }
}
