using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager
{
    private List<InventoryItemInfos> itemsInInventory;
    private InventoryManager inventory;
    private CraftingUI craftingUI;

    public CraftingManager(CraftingUI craftingUI, InventoryManager inventory)
    {
        SetCraftingUI(craftingUI);
        SetInventory(inventory);
    }

    public void SetCraftingUI(CraftingUI craftingUI)
    {
        this.craftingUI = craftingUI;
        craftingUI.GenerateCompleteUI(this);
    }

    public void SetInventory(InventoryManager inventory) 
    {
        this.inventory = inventory;
        UpdateInventoryList();
    }


    public void UpdateInventoryList() { itemsInInventory = inventory.GetItemsInInventory(); }

    public void CraftItem(CraftingRecipes_SO recipe)
    {
        foreach (CraftingCost ingredient in recipe.craftRequirements)
        {
            if (!ingredient.isTool) { inventory.RemoveItemFromInventory(ingredient.item, ingredient.itemAmount); } 
            else { Debug.Log("Tool "+ingredient.item.name); }
        }
        inventory.AddItemToInventory(recipe.resultItem, recipe.amountCrafted);
    }

    public bool CanCraftItem(CraftingRecipes_SO recipe)
    {
        // Aggregate craftRequirements array in case of duplicated items to check for amounts in inventory
        List<InventoryItemInfos> aggregatedCraftRequirements = new List<InventoryItemInfos>();
        foreach (CraftingCost ingredient in recipe.craftRequirements)
        { 
            if (aggregatedCraftRequirements.Exists(ing => ing.itemSO == ingredient.item)) 
            { 
                aggregatedCraftRequirements.Find(ing => ing.itemSO == ingredient.item).itemAmount += ingredient.itemAmount; 
            }
            else 
            {
                aggregatedCraftRequirements.Add(new InventoryItemInfos(ingredient.item, ingredient.itemAmount)); 
            }
        }
        foreach (InventoryItemInfos ingredient in aggregatedCraftRequirements)
        {
            int amountInInventory = inventory.AmountOfItemInInventory(ingredient.itemSO);
            if (amountInInventory < ingredient.itemAmount) { return false; }
        }
        return true;
    }

}
