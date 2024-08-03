using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CraftingPanel")]
public class CraftingPanel_SO : ScriptableObject
{

    public string title;
    public Sprite panelIcon;
    public CraftingRecipes_SO[] craftingRecipes; 
}
