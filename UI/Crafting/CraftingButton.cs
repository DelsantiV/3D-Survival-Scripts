
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour
{
    private CraftingRecipes_SO craftingRecipe;
    private GameObject craftButtonHolder;
    private Image outputImageHolder;
    private Item_General_SO resultItem;
    private CraftingCost[] totalCraftingCost;
    private GameObject ingredientText;
    private TextMeshProUGUI titleText;

    private CraftingManager craftingManager;

    public void SetCraftingButton(CraftingManager craftingManager, CraftingRecipes_SO craftingRecipe)
    {
        SetCraftingManager(craftingManager);
        this.craftingRecipe = craftingRecipe;
        craftButtonHolder = transform.Find("CraftButton").gameObject;
        Sprite outputImage = craftingRecipe.resultItem.iconInInventory;
        outputImageHolder = transform.Find("Image").Find("OutputImage").GetComponent<Image>();
        outputImageHolder.sprite = outputImage;
        resultItem = craftingRecipe.resultItem;
        totalCraftingCost = craftingRecipe.craftRequirements;
        ingredientText = transform.Find("IngredientText").gameObject;
        titleText = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        titleText.SetText(craftingRecipe.resultItem.itemName);

        SetIngredients();
        CustomTickSystem.OnTick += UpdateButton;
    }

    private void SetIngredients()
    {
        foreach(CraftingCost craftRequirement in totalCraftingCost) 
        {
            Item_General_SO ingredient = craftRequirement.item;
            int amount = craftRequirement.itemAmount;
            if (ingredientText.GetComponent<TextMeshProUGUI>().text == "")
            {
                ingredientText.GetComponent<TextMeshProUGUI>().SetText(ingredient.itemName + " : " + amount.ToString());
            }
            else
            {
                
                ingredientText.GetComponent<TextMeshProUGUI>().SetText(ingredientText.GetComponent<TextMeshProUGUI>().text + "\n" + ingredient.itemName + " : " + amount.ToString());
                
            }
            if (craftRequirement.isTool)
            {
                ingredientText.GetComponent<TextMeshProUGUI>().SetText(ingredientText.GetComponent<TextMeshProUGUI>().text + " (Tool)");

            }
        }
    }

    public void SetCraftingManager(CraftingManager craftingManager) { this.craftingManager = craftingManager; }

    public void CraftItem() 
    {
        craftingManager.CraftItem(craftingRecipe);
        Debug.Log("Crafting Item !");
    }

    private bool CanCraftItem()
    {
        return craftingManager.CanCraftItem(craftingRecipe);
    }

    private void UpdateButton()
    {
        if (craftingManager != null)
        {
            if (CanCraftItem())
            {
                craftButtonHolder?.SetActive(true);
            }
            else
            {
                craftButtonHolder?.SetActive(false);
            }
        }
    }
}
