using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingsRecipe", menuName = "ScriptableObjects/CraftingRecipe")]
public class CraftingRecipes_SO : ScriptableObject
{
    public Item_General_SO resultItem;
    public int amountCrafted;
    public CraftingCost[] craftRequirements;

}
