using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class Crafting
    {
        public enum CraftingAction
        {
            Strech,
            Crush,
            Assemble,
            Mix,
            Hit
        }

        public enum CraftingProcess
        {
            Cook,
            Burn,
            Dry,
            Moisten,
            Grind
        }

    }
    public enum CraftingTable
    {
        FirePit
    }

    public class CraftingRecipe
    {
        public Crafting.CraftingAction craftingAction;
        public ItemPile firstItemPile;
        public ItemPile secondItemPile;
        public bool isSecondPileTool;
        public ItemPile resultItemPile;
        public bool requiresTwoHands;
        public bool isSecondPileNeeded
        {
            get { return !secondItemPile.IsEmpty; }
        }

        public CraftingRecipe(JObject jsonFile) 
        {
            craftingAction = (Crafting.CraftingAction) Enum.Parse(typeof(Crafting.CraftingAction), jsonFile["craftingAction"].ToString());
            firstItemPile = new ItemPile(JsonConvert.DeserializeObject<List<string>>(jsonFile["firstItemPile"].ToString()));
            secondItemPile = new();
            if (jsonFile["secondItemPile"] != null)
            { if (jsonFile["secondItemPile"].ToString() != "")
                {
                    secondItemPile = new ItemPile(JsonConvert.DeserializeObject<List<string>>(jsonFile["secondItemPile"].ToString()));
                }
            }
            else secondItemPile = null;
            if (jsonFile["isSecondItemPileTool"] != null) isSecondPileTool = JsonConvert.DeserializeObject<bool>(jsonFile["isSecondPileTool"].ToString());
            resultItemPile = new ItemPile(JsonConvert.DeserializeObject<List<string>>(jsonFile["resultItemPile"].ToString()));
            if (jsonFile["requiresTwoHands"] != null) isSecondPileTool = JsonConvert.DeserializeObject<bool>(jsonFile["requiresTwoHands"].ToString());
            else requiresTwoHands = false;

            Debug.Log("Loaded recipe : " + ToString());
        }

        public override string ToString()
        {
            string ingredient = firstItemPile.ToString() + " ";
            if (isSecondPileNeeded)
            {
                ingredient += "and " + secondItemPile.ToString() + " ";
            }
            return ingredient + "with action " + craftingAction + " gives " + resultItemPile;
        }
    }

    public class CraftingProcessRecipe
    {
        public Crafting.CraftingProcess craftingProcess;
        public ItemPile itemPile;
        public World.World.EnvConditions EnvConditions;

        public CraftingProcessRecipe(JObject jsonFile)
        {

        }
    }
}