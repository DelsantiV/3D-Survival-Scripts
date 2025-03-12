using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GoTF.Content.Crafting;

namespace GoTF.Content
{
    public class CraftingManager
    {
        private readonly PlayerManager player;
        private HandsInventory HandsInventory
        {
            get { return player.HandsInventory; }
        }
        private ItemPile PrefHandPile
        {
            get 
            {
                return HandsInventory.PrefHandPile; 
            }
        }
        private ItemPile OtherHandPile
        {
            get { return HandsInventory.OtherHandPile; }
        }
        public Dictionary<CraftingAction, List<CraftingRecipe>> possibleCraftingRecipesByAction;
        public CraftingManager(PlayerManager player)
        {
            this.player = player;
            possibleCraftingRecipesByAction = new();
        }

        public void UpdateCraftingRecipes()
        {
            possibleCraftingRecipesByAction.Clear();
            foreach (CraftingAction craftingAction in CraftingRecipesUtilities.allCraftingRecipesByAction.Keys)
            {
                possibleCraftingRecipesByAction[craftingAction] = PossibleCraftingRecipesForAction(craftingAction);
            }
        }

        public List<CraftingRecipe> PossibleCraftingRecipesForAction(CraftingAction craftingAction)
        {
            return CraftingRecipesUtilities.GetAllRecipesForTwoPilesAndAction(craftingAction, PrefHandPile, OtherHandPile);
        }

        public void PerformCraftingAction(CraftingAction craftingAction)
        {
            Debug.Log("Doing craft " + craftingAction);
        }

        public void PerformCraftingRecipe(CraftingRecipe recipe)
        {
            ItemPile firstItemPile;
            ItemPile secondItemPile;
            int recipeConfig = IsRecipePossible(recipe);
            if (recipeConfig == 0) return;
            if (recipeConfig == 1)
            {
                firstItemPile = PrefHandPile;
                secondItemPile = OtherHandPile;
            }
            else
            {
                firstItemPile = OtherHandPile;
                secondItemPile = PrefHandPile;
            }
            HandsInventory.TryRemoveItemPile(firstItemPile, true);
            if (recipe.isSecondPileNeeded && !recipe.isSecondPileTool)
            {
                HandsInventory.TryRemoveItemPile(secondItemPile, true);
            }
            HandsInventory.TryAddItemPileToNextHand(recipe.resultItemPile);
        }

        public int IsRecipePossible(CraftingRecipe recipe)
        {
            if (PrefHandPile.Contains(recipe.firstItemPile))
            {
                if (OtherHandPile.Contains(recipe.secondItemPile)) return 1;
            }
            if (PrefHandPile.Contains(recipe.secondItemPile))
            {
                if (OtherHandPile.Contains(recipe.firstItemPile)) return 2;
            }
            return 0;
        }
    }

    public class CraftingRecipesUtilities
    {
        public static Dictionary<CraftingAction, List<CraftingRecipe>> allCraftingRecipesByAction;
        public static Dictionary<CraftingProcess, List<CraftingProcessRecipe>> allCraftingProcessesByProcess;
        public static List<CraftingRecipe> AllCraftingRecipes
        {
            get { return allCraftingRecipesByAction.Values.ToList().SelectMany(recipe => recipe).ToList(); }
        }
        public static List<CraftingProcessRecipe> AllCraftingProcesses
        {
            get { return allCraftingProcessesByProcess.Values.ToList().SelectMany(recipe => recipe).ToList(); }
        }

        public static List<CraftingRecipe> GetAllRecipesForAction(CraftingAction craftAction) 
        {
            if (allCraftingRecipesByAction.ContainsKey(craftAction)) return allCraftingRecipesByAction[craftAction];
            else return new();
        }
        public static List<CraftingRecipe> GetAllRecipesForPile(ItemPile pile, List<CraftingRecipe> craftingRecipes = null) 
        { 
            if (pile == null) { return new(); }
            if (pile.IsEmpty) { return new(); }
            if (craftingRecipes == null) craftingRecipes = AllCraftingRecipes;
            return craftingRecipes.FindAll(recipe => (ItemPilesUtilities.ArePileCorresponding(recipe.firstItemPile,pile) | ItemPilesUtilities.ArePileCorresponding(recipe.secondItemPile, pile))); 
        }

        public static List<CraftingRecipe> GetAllRecipesForTwoPiles(ItemPile pile1, ItemPile pile2, List<CraftingRecipe> craftingRecipes = null)
        {
            List<CraftingRecipe> pile1Recipes = GetAllRecipesForPile(pile1, craftingRecipes);
            List<CraftingRecipe> pile2Recipes = GetAllRecipesForPile(pile2, craftingRecipes);
            return pile1Recipes.Intersect(pile1Recipes).ToList();
        }

        public static List<CraftingRecipe> GetAllRecipesForTwoPilesAndAction(CraftingAction craftingAction, ItemPile pile1, ItemPile pile2)
        {
            return GetAllRecipesForTwoPiles(pile1, pile2, GetAllRecipesForAction(craftingAction));

        }
    }
}
