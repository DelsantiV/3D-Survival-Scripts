using UnityEngine;
using GoTF.Content;
using GoTF.Utilities;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json.Linq;

namespace GoTF.GameLoading
{
    public class CraftingRecipesLoader
    {
        public List<CraftingRecipe> AllCraftingRecipes;
        public List<CraftingProcess> AllCraftingProcesses;
        private readonly List<string> recipesJsonKeys = new() { "JSON", "Recipes" };

        public IEnumerator LoadRecipes()
        {
            AllCraftingRecipes = new();
            AllCraftingProcesses = new();
            Debug.Log("Start retrieving crafting recipes...");
            AsyncOperationHandle<IList<TextAsset>> recipesLoading = Addressables.LoadAssetsAsync<TextAsset>(recipesJsonKeys,
                recipeJSON => { RegisterRecipe(recipeJSON); },
                Addressables.MergeMode.Intersection);
            yield return recipesLoading;
            CraftingRecipes.allCraftingRecipes = AllCraftingRecipes;
            CraftingRecipes.allCraftingProcesses = AllCraftingProcesses;
            Addressables.Release(recipesLoading);
        }

        private void RegisterRecipe(TextAsset recipeJSON)
        {
            if (IsValidRecipe(recipeJSON)) AllCraftingRecipes.Add(new CraftingRecipe(JObject.Parse(recipeJSON.text)));
        }

        private bool IsValidRecipe(TextAsset recipeJSON)
        {
            JObject recipe = JObject.Parse(recipeJSON.text);
            return JSONUtilities.DoesJSONContainField(recipe, new List<string>() { "craftingAction", "firstItemPile", "resultItemPile"}); 
        }
    }
}
