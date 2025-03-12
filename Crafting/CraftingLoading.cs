using UnityEngine;
using GoTF.Content;
using GoTF.Utilities;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json.Linq;
using static GoTF.Content.Crafting;
using System;

namespace GoTF.GameLoading
{
    public class CraftingRecipesLoader
    {
        public Dictionary<CraftingAction, List<CraftingRecipe>> AllCraftingRecipesByAction;
        public Dictionary<CraftingProcess, List<CraftingProcessRecipe>> AllCraftingProcessesByProcess;
        private readonly List<string> recipesJsonKeys = new() { "JSON", "Recipes" };

        public IEnumerator LoadRecipes()
        {
            AllCraftingRecipesByAction = new();
            AllCraftingProcessesByProcess = new();
            Debug.Log("Start retrieving crafting recipes...");
            AsyncOperationHandle<IList<TextAsset>> recipesLoading = Addressables.LoadAssetsAsync<TextAsset>(recipesJsonKeys,
                recipeJSON => { RegisterRecipe(recipeJSON); },
                Addressables.MergeMode.Intersection);
            yield return recipesLoading;
            CraftingRecipesUtilities.allCraftingRecipesByAction = AllCraftingRecipesByAction;
            CraftingRecipesUtilities.allCraftingProcessesByProcess = AllCraftingProcessesByProcess;
            Addressables.Release(recipesLoading);
        }

        private void RegisterRecipe(TextAsset recipeJSON)
        {
            if (IsValidRecipe(recipeJSON))
            {
                JObject jsonFile = JObject.Parse(recipeJSON.text);
                CraftingAction craftingAction = (CraftingAction)Enum.Parse(typeof(CraftingAction), jsonFile["craftingAction"].ToString());
                if (AllCraftingRecipesByAction.ContainsKey(craftingAction)) { AllCraftingRecipesByAction[craftingAction].Add(new CraftingRecipe(jsonFile)); }
                else AllCraftingRecipesByAction.Add(craftingAction, new List<CraftingRecipe>() { new CraftingRecipe(jsonFile) });
            }
        }

        private bool IsValidRecipe(TextAsset recipeJSON)
        {
            JObject recipe = JObject.Parse(recipeJSON.text);
            return JSONUtilities.DoesJSONContainField(recipe, new List<string>() { "craftingAction", "firstItemPile", "resultItemPile"}); 
        }
    }
}
